using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MauiApp_AnyThingLM_RAG.Models;
using MauiApp_AnyThingLM_RAG.Utils;
using MauiApp_IA_IOT.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MauiApp_AnyThingLM_RAG.Managers
{
    public class AnyThingLLManager
    {
        private HttpClient _httpClient;
        private string _baseUrl;

        public WorkspaceRoot WorkspaceRoot { get; set; }

        public AnyThingLLManager(string baseUrl, string apiKey)
        {
            this._httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            this._baseUrl = baseUrl;
            this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            this._httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //  CHAT
        public async Task<dynamic> SendMessageAsync(string message, string systemPrompt, double temperature, string maxTokens, string chatMode, string slug)
        {
            dynamic objResult = null;
            try
            {
                var payload = new
                {
                    message = message,
                    mode = chatMode.ToLower(),
                    sessionId = Guid.NewGuid().ToString(),
                    attachments = new object[0],
                    systemPrompt = systemPrompt,
                    maxTokens = maxTokens,
                    temperature = temperature,
                };

                string jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                string url = UrlUtils.GetFullUrl(this._baseUrl, $"/workspace/{slug}/chat");
                HttpResponseMessage response = await this._httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                    objResult = await GetInfoChunks(response);
                else
                {
                    objResult = new
                    {
                        Error = new
                        {
                            Message = "No se ha podido obtener respuesta"
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                objResult = new
                {
                    Error = new
                    {
                        Message = ex.Message
                    }
                };
            }

            return objResult;
        }
        private async Task<dynamic> GetInfoChunks(HttpResponseMessage response)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseContent);

            string menssageResponse = (string)result.textResponse;
            List<Source> sources = (result.sources as JArray).ToObject<List<Source>>();
            Dictionary<string, List<string>> reference = MessageReferenceUtils.GetReferenceDocument(sources);

            var objResult = new
            {
                Data = new
                {
                    Text = menssageResponse,
                    Refs = reference
                }
            };

            return objResult;
        }

        //  DOCUMENT
        public async Task<dynamic> TakeDocumentAsync(string slug)
        {
            dynamic result = null;
            try
            {
                var fileResult = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecciona un documento",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".pdf", ".docx", ".txt", ".csv", ".json" } },
                        { DevicePlatform.Android, new[] { "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain", "text/csv" } },
                        { DevicePlatform.iOS, new[] { "public.pdf", "com.microsoft.word.doc", "public.plain-text", "public.comma-separated-values-text" } },
                    })
                });

                if (fileResult != null)
                {
                    string filePath = fileResult.FullPath;

                    // 1. Subir el documento
                    dynamic document = await UploadDocument(new FileResult(filePath), slug);
                    if (document == null)
                    {
                        result = new
                        {
                            Error = new
                            {
                                Message = "El documento no se ha podido subir. Inténtelo de nuevo."
                            }
                        };
                        return result;
                    }

                    string location = document.location;
                    string title = document.title;
                    string fileName = location.Substring(location.LastIndexOf('/') + 1);

                    // 2. Mover el documento al workspace.
                    bool moved = await MoveDocument(location, fileName);
                    if (!moved)
                    {
                        result = new
                        {
                            Error = new
                            {
                                Message = "El documento no ha sido movido al workspace"
                            }
                        };
                        return result;
                    }

                    // 3. Actualizar embeddings
                    bool updated = await UpdateEmbeddings(fileName, slug);
                    if (updated)
                    {
                        result = new
                        {
                            Response = new
                            {
                                Message = "El embedding ha sido modificado con éxito!"
                            }
                        };
                    }
                    else
                    {
                        result = new
                        {
                            Error = new
                            {
                                Message = "El embedding no ha podido ser modificado."
                            }
                        };
                    }
                }
                else
                {
                    result = new
                    {
                        Error = new
                        {
                            Message = "No se ha seleccionado ningún documento"
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                result = new
                {
                    Error = new
                    {
                        Message = ex.Message
                    }
                };
            }
            return result;
        }
        private async Task<bool> MoveDocument(string location, string toName)
        {
            string workspaceFolder = "custom-documents/";

            var movePayload = new
            {
                files = new[]
                {
                    new
                    {
                        from = location,
                        to = $"{workspaceFolder}{toName}"
                    }
                }
            };

            string moveJson = JsonConvert.SerializeObject(movePayload);
            var moveContent = new StringContent(moveJson, Encoding.UTF8, "application/json");
            HttpResponseMessage moveResponse = await this._httpClient.PostAsync($"/document/move-files", moveContent);

            return moveResponse.IsSuccessStatusCode;
        }
        private async Task<dynamic> UploadDocument(FileResult fileResult, string slug)
        {
            string originalFileName = fileResult.FileName;

            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new StringContent(slug), "slug");

            using (Stream fileStream = await fileResult.OpenReadAsync())
            {
                // Lee los bytes desde el Stream
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();

                    ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    form.Add(fileContent, "file", originalFileName);

                    string url = UrlUtils.GetFullUrl(this._baseUrl, "/document/upload");
                    HttpResponseMessage uploadResponse = await this._httpClient.PostAsync(url, form);

                    if (uploadResponse.IsSuccessStatusCode)
                    {
                        string uploadResult = await uploadResponse.Content.ReadAsStringAsync();
                        dynamic uploadJson = JsonConvert.DeserializeObject(uploadResult);
                        return uploadJson.documents[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        private async Task<bool> UpdateEmbeddings(string fileName, string slug)
        {
            var updatePayload = new
            {
                adds = new[] { "custom-documents/" + fileName },
                deletes = new string[0]
            };

            string updateJson = JsonConvert.SerializeObject(updatePayload);
            var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");

            string url = UrlUtils.GetFullUrl(this._baseUrl, $"/workspace/{slug}/update-embeddings");
            HttpResponseMessage updateResponse = await this._httpClient.PostAsync(url, updateContent);
            return updateResponse.IsSuccessStatusCode;
        }
        public async Task<dynamic> TakeWorkspaceDocumentsAsync(string slug)
        {
            dynamic result = null;
            try
            {
                result = await GetAllDocuments(slug);
            }
            catch (Exception ex)
            {
                result = new
                {
                    Error = new
                    {
                        Message = ex.Message
                    }
                };
            }
            return result;
        }
        private async Task<dynamic> GetAllDocuments(string slug)
        {
            dynamic result = null;
            string url = UrlUtils.GetFullUrl(this._baseUrl, $"/workspace/{slug}");
            HttpResponseMessage response = await this._httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                string contentFormatted = responseContent.Substring(0, 11) + $"s\":" + responseContent.Substring(12 + 1);

                //  Obtenemos los datos del workspace
                this.WorkspaceRoot = JsonConvert.DeserializeObject<WorkspaceRoot>(contentFormatted);

                Workspace workspace = this.WorkspaceRoot.Workspaces.Where(w => w.Slug == slug).FirstOrDefault();

                //  Obtenemos la lista de documentos
                List<Metadata> documents = new List<Metadata>();
                foreach(Document doc in workspace.Documents)
                {
                    //  Obtenemos los metadatos del documento
                    var metadata = JsonConvert.DeserializeObject<Metadata>(doc.Metadata);

                    //  Añadimos el documento a la lista
                    documents.Add(metadata);
                }

                //  Guardamos el resultado
                result = new
                {
                    Data = documents
                };
            }
            else
            {
                result = new
                {
                    Error = new
                    {
                        Message = "No se ha podido obtener la lista de documentos"
                    }
                };
            }
            return result;
        }
    
        //  WORKSPACES
        public async Task<WorkspaceRoot> GetAllWorkSpaces()
        {
            List<Workspace> workspaces = new List<Workspace>();
            try
            {
                string url = UrlUtils.GetFullUrl(this._baseUrl, "/workspaces");
                HttpResponseMessage response = await this._httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    this.WorkspaceRoot = JsonConvert.DeserializeObject<WorkspaceRoot>(responseContent);
                }
            }
            catch (Exception ex)
            {
                GuiUtils.SendSnakbarMessage(ex.Message);
            }

            return this.WorkspaceRoot;
        }
        public async Task<Workspace> CreateNewWorkspaceAsync(string workspaceName)
        {
            Workspace workspace = null;
            try
            {
                var payload = new
                {
                    name = workspaceName,
                    similarityThreshold = 0.7,
                    openAiTemp = 0.7,
                    openAiHistory = 20,
                    openAiPrompt = "Custom prompt for responses",
                    queryRefusalResponse = "Custom refusal message",
                    chatMode = "chat",
                    topN = 4
                };

                string jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                string url = UrlUtils.GetFullUrl(this._baseUrl, "/workspace/new");
                HttpResponseMessage response = await this._httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    workspace = JsonConvert.DeserializeObject<WorkspaceResponse>(responseContent).Workspace;
                }
            }
            catch (Exception ex)
            {
                workspace = null;
            }

            return workspace;
        }
        public async Task<dynamic> DeleteWorkspaceAsync(string slug)
        {
            dynamic objResult = null;
            try
            {
                string url = UrlUtils.GetFullUrl(this._baseUrl, $"/workspace/{slug}");
                HttpResponseMessage response = await this._httpClient.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                    objResult = new
                    {
                        Data = "Se ha eliminado el workspace"
                    };
                else
                {
                    objResult = new
                    {
                        Error = new
                        {
                            Message = "No se ha podido obtener respuesta"
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                objResult = new
                {
                    Error = new
                    {
                        Message = ex.Message
                    }
                };
            }

            return objResult;
        }

        //  THREAD
        public async Task<ConversationHistory>? GetThreadMessagesAsync(string workspaceSlug, string threadSlug)
        {
            ConversationHistory conversation = null;

            try
            {
                string url = UrlUtils.GetFullUrl(this._baseUrl, $"/workspace/{workspaceSlug}/thread/{threadSlug}/chats");
                HttpResponseMessage response = await this._httpClient.GetAsync(url);
                string responseContent = await response.Content.ReadAsStringAsync();
                conversation = JsonConvert.DeserializeObject<ConversationHistory>(responseContent);
            }
            catch (Exception ex)
            {
                conversation = null;
            }

            return conversation;
        }
        public async Task<Models.Thread> CreateNewThread(string workspaceSlug, string threadName)
        {
            Models.Thread thread = null;
            try
            {
                string url = UrlUtils.GetFullUrl(this._baseUrl, $"/workspace/{workspaceSlug}/thread/new");

                var payload = new{name=threadName};
                string jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await this._httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    thread = JsonConvert.DeserializeObject<ThreadResponse>(responseContent).Thread;
                }
            }
            catch (Exception ex)
            {
                thread = null;
            }

            return thread;
        }
        public async Task<dynamic> DeleteThread(string slug, string threadSlug)
        {
            dynamic objResult = null;
            try
            {
                string url = UrlUtils.GetFullUrl(this._baseUrl, $"/workspace/{slug}/thread/{threadSlug}");
                HttpResponseMessage response = await this._httpClient.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    objResult = new
                    {
                        Data = "Se ha eliminado el hilo correctamente"
                    };
                }
                else
                {
                    objResult = new
                    {
                        Error = new
                        {
                            Message = "No se ha podido obtener respuesta"
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                objResult = new
                {
                    Error = new
                    {
                        Message = ex.Message
                    }
                };
            }

            return objResult;
        }
    }
}

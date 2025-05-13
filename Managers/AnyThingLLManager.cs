using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using MauiApp_AnyThingLM_RAG.Models;
using MauiApp_AnyThingLM_RAG.Utils;
using Newtonsoft.Json;

namespace MauiApp_AnyThingLM_RAG.Managers
{
    public class AnyThingLLManager
    {
        private string _baseUrl;
        private string _apiKey;

        public WorkspaceRoot WorkspaceRoot { get; set; }

        public AnyThingLLManager(string baseUrl, string apiKey)
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
        }

        //  CHAT
        public async Task<dynamic> SendMessageAsync(string message, string systemPrompt, string chatMode, string slug)
        {
            dynamic objResult = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);

                    var payload = new
                    {
                        message = message,
                        mode = chatMode.ToLower(),
                        sessionId = Guid.NewGuid().ToString(),
                        attachments = new object[0],
                        systemPrompt = systemPrompt
                    };

                    string jsonPayload = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{_baseUrl}/workspace/{slug}/chat", content);
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
        private Dictionary<string, List<string>> GetReferenceDocument(dynamic sources)
        {
            Dictionary<string, List<string>> references = new Dictionary<string, List<string>>();
            if (sources != null && sources.Count > 0)
            {
                foreach (var source in sources)
                {
                    string text = Regex.Replace(source["text"].ToString(), @"[\r\n]+", " ");

                    //  Get the document
                    int startIndex = text.IndexOf("sourceDocument: ") + "sourceDocument: ".Length;
                    int endIndex = text.IndexOf(" published:");
                    string sourceDocument = text.Substring(startIndex, endIndex - startIndex);

                    //  Get the reference
                    int indexOfStart = text.IndexOf("</document_metadata> ") + "</document_metadata> ".Length;
                    string reference = text.Substring(indexOfStart).Trim();

                    //  Add the reference to the dictionary
                    if (!references.ContainsKey(sourceDocument))
                    {
                        references[sourceDocument] = new List<string>();
                    }

                    references[sourceDocument].Add(reference);
                }
            }

            return references;
        }
        private async Task<dynamic> GetInfoChunks(HttpResponseMessage response)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseContent);

            string menssageResponse = (string)result.textResponse;
            Dictionary<string, List<string>> reference = GetReferenceDocument(result.sources);

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

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);

                        // 1. Subir el documento
                        dynamic document = await UploadDocument(client, new FileResult(filePath), slug);
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
                        bool moved = await MoveDocument(client, location, fileName);
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
                        bool updated = await UpdateEmbeddings(client, fileName, slug);
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
        private async Task<bool> MoveDocument(HttpClient client, string location, string toName)
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
            HttpResponseMessage moveResponse = await client.PostAsync($"{this._baseUrl}/document/move-files", moveContent);

            return moveResponse.IsSuccessStatusCode;
        }
        private async Task<dynamic> UploadDocument(HttpClient client, FileResult fileResult, string slug)
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

                    HttpResponseMessage uploadResponse = await client.PostAsync($"{this._baseUrl}/document/upload", form);

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
        private async Task<bool> UpdateEmbeddings(HttpClient client, string fileName, string slug)
        {
            var updatePayload = new
            {
                adds = new[] { "custom-documents/" + fileName },
                deletes = new string[0]
            };

            string updateJson = JsonConvert.SerializeObject(updatePayload);
            var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");
            HttpResponseMessage updateResponse = await client.PostAsync(
                $"{this._baseUrl}/workspace/{slug}/update-embeddings", updateContent);
            return updateResponse.IsSuccessStatusCode;
        }

        //  GET DOCUMENTS
        public async Task<dynamic> TakeWorkspaceDocumentsAsync(string slug)
        {
            dynamic result = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);
                    result = await GetAllDocuments(client, slug);
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
        private async Task<dynamic> GetAllDocuments(HttpClient client, string slug)
        {
            dynamic result = null;
            HttpResponseMessage response = await client.GetAsync($"{this._baseUrl}/workspace/{slug}");
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                string contentFormatted = responseContent.Substring(0, 11) + $"s\":" + responseContent.Substring(12 + 1);

                //  Obtenemos los datos del workspace
                this.WorkspaceRoot = JsonConvert.DeserializeObject<WorkspaceRoot>(contentFormatted);

                //  Obtenemos la lista de documentos
                List<Metadata> documents = new List<Metadata>();
                foreach(Document doc in this.WorkspaceRoot.Workspaces[0].Documents)
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
                using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);

                    string uri = $"{this._baseUrl}/workspaces";
                    HttpResponseMessage response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        this.WorkspaceRoot = JsonConvert.DeserializeObject<WorkspaceRoot>(responseContent);
                    }
                }
            }
            catch (Exception ex)
            {
                GuiUtils.SendSnakbarMessage(ex.Message);
            }

            return this.WorkspaceRoot;
        }
        public async Task<dynamic> GetChatsWorkspace(string slug)
        {
            dynamic objResult = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);

                    HttpResponseMessage response = await client.GetAsync($"{_baseUrl}/workspace/{slug}/chats");
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        objResult = new
                        {
                            Data = responseJson
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
        public async Task<dynamic> CreateNewWorkspace(string slug)
        {
            dynamic objResult = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);

                    var payload = new
                    {
                        name = slug,
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

                    HttpResponseMessage response = await client.PostAsync($"{_baseUrl}/workspace/new", content);
                    if (response.IsSuccessStatusCode)
                        objResult = new
                        {
                            Data = "Se ha creado el workspace"
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
        public async Task<dynamic> DeleteWorkspace(string slug)
        {
            dynamic objResult = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);

                    HttpResponseMessage response = await client.DeleteAsync($"{this._baseUrl}/workspace/{slug}");
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
        public async Task<dynamic> CreateNewThread(string slug)
        {
            dynamic objResult = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);

                    HttpResponseMessage response = await client.GetAsync($"{_baseUrl}/workspace/{slug}/thread/new");
                    if (response.IsSuccessStatusCode)
                    {
                        objResult = new
                        {
                            Data = "Se ha creado el hilo correctamente"
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
        public async Task<dynamic> DeleteThread(string slug, string threadSlug)
        {
            dynamic objResult = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);

                    HttpResponseMessage response = await client.DeleteAsync($"{_baseUrl}/workspace/{slug}/thread/{threadSlug}");
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

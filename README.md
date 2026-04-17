# 🤖 AI Programming – Hishtalmot Branch

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![OpenAI SDK](https://img.shields.io/badge/OpenAI-SDK-412991?logo=openai&logoColor=white)](https://github.com/openai/openai-dotnet)
[![Gemini SDK](https://img.shields.io/badge/Google-Gemini%20SDK-4285F4?logo=google&logoColor=white)](https://ai.google.dev/)
[![Razor Pages](https://img.shields.io/badge/ASP.NET-Razor%20Pages-blueviolet)](https://learn.microsoft.com/aspnet/core/razor-pages/)

Created by **[Gilad Markman](https://webprogramming.azurewebsites.net/)**

---

<a id="overview"></a>
## 📋 Overview

This branch is a step-by-step, hands-on course covering how to call modern LLMs from **.NET 8** using the official vendor SDKs:

- **OpenAI .NET SDK** (Chat Completions + the new **Responses API**)
- **Google Gemini .NET SDK**

Each lesson is a standalone console app (or WinForms / Razor Pages app) that isolates a single concept — stateless calls, history, prompting, streaming, structured output, tool calling, image & voice generation, embeddings, and RAG.

> **Note:** The repository is named **SK** because the `main` branch is built around **Microsoft Semantic Kernel**.
> This **`Hishtalmot`** branch takes a different path: after the first lesson, it uses the **OpenAI SDK** and **Google Gemini SDK** directly — no Semantic Kernel wrapper — to keep things closer to the raw model APIs.

---

## 📚 Table of Contents

- [Overview](#overview)
- [Slides](#slides)
- [Video Playlist](#video-playlist)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Environment Setup](#environment-setup)
- [Lessons](#lessons)
  - [Lesson 1 – Introduction](#lesson-1)
  - [Lesson 2 – Stateless Calls](#lesson-2)
  - [Lesson 3 – Chat History](#lesson-3)
  - [Lesson 4 – Prompts](#lesson-4)
  - [Lesson 5 – Model Settings](#lesson-5)
  - [Lesson 6 – Streaming](#lesson-6)
  - [Lesson 7 – Structured Output (JSON)](#lesson-7)
  - [Lesson 8 – Tools / Function Calling / ReAct Loop](#lesson-8)
  - [Lesson 9 – Images & Voice](#lesson-9)
  - [Lesson 10 – Embeddings](#lesson-10)
  - [Lesson 11 – RAG](#lesson-11)
  - [Lesson 12 – Razor Pages UI](#lesson-12)
  - [WinForm Projects](#winforms)
- [License](#license)

---

<a id="slides"></a>
## 🎞️ Slides

PowerPoint presentations for this tutorial are available here:
[**📂 Download from Dropbox**](https://www.dropbox.com/scl/fo/upt25ke47aqeprnccitla/AEdxFqKDXbm_cGPXEY_w8h4?rlkey=v58m7u94svuz3owii4xjigft4&dl=0)

<a id="video-playlist"></a>
## 🎥 Video Playlist

Watch the full step-by-step tutorial on YouTube:
[**▶️ AI Programming Playlist**](https://www.youtube.com/playlist?list=PLDDjraqDVBZYs5J5PCp_KSXCeP29JzYuM)

---

<a id="tech-stack"></a>
## 🧰 Tech Stack

- **.NET 8**, C# 12
- **OpenAI SDK** – chat, responses, embeddings, images, TTS
- **Google Gemini SDK** – chat, embeddings, image generation
- **DotNetEnv** – `.env` loading for API keys
- **Tavily API** – web search tool
- **Pinecone** – vector database for RAG
- **UglyToad.PdfPig / iText7** – PDF parsing
- **SQL Server (LocalDB)** – `AI_Programming.mdf` used by SQL tools
- **ASP.NET Core Razor Pages** & **WinForms** – UI front-ends

<a id="getting-started"></a>
## 🚀 Getting Started

```powershell
git clone https://github.com/MarkmanGilad/SK.git
cd SK
git checkout Hishtalmot
```

Open `SK.sln` in Visual Studio 2022/2026, set the lesson you want to run as the **Startup Project**, and press F5.

<a id="environment-setup"></a>
## 🔐 Environment Setup

> ⚠️ **If a `.env` file already exists in the repo root, delete it first and create a fresh one** from `.env_example`. This avoids stale or mismatched keys from a previous setup.

Copy `.env_example` to `.env` in the repo root and fill in your keys:

```
OpenAIKey=sk-...
OPENAI_API_KEY=sk-...
GEMINI_API_KEY=...
TAVILY_API_KEY=...
PINECONE_API_KEY=...
```

The projects use `DotNetEnv` with `Env.TraversePath().Load()`, so any `.env` up the folder tree will be picked up.

---

<a id="lessons"></a>
## 📖 Lessons

<a id="lesson-1"></a>
### 👋 Lesson 1 – Introduction (`Lesson_1_intro`)
First contact with the models. Shows **multiple ways** of making the same call so you can compare them side-by-side:

- `OpenAI_Http.cs` – raw `HttpClient` call to the OpenAI REST endpoint
- `OpenAI_SDK.cs` – same call via the **OpenAI SDK** (Chat Completions)
- `OpenAI_SDK_Response.cs` – using the newer **Responses API**
- `OpenAI_SK.cs` – for comparison, using **Semantic Kernel**
- `Gemini_Http.cs` / `Gemini_SDK.cs` / `Gemini_SK.cs` – the same three flavors for Google Gemini
- `Tokens.cs` – tokenizer demo (counting tokens)

> This is the only lesson that still shows the SK flavor. From Lesson 2 onward the branch sticks to the vendor SDKs.

<a id="lesson-2"></a>
### 🎯 Lesson 2 – Stateless Calls (`Lesson_2_StateLess`)
Demonstrates that a plain completion call has **no memory**. Each request is independent — the model does not remember previous turns. Separate files for OpenAI Chat Completions, OpenAI Responses, and Gemini.

<a id="lesson-3"></a>
### 💬 Lesson 3 – Chat History (`Lesson_3_History`)
Adds state by maintaining a **message history** list and resending it every turn. Shows both the OpenAI Chat/Responses flow and the Gemini flow. Includes a small server-side variant (`OpenAI_SDK_Response_srv.cs`) showing how the Responses API can persist state server-side via `previous_response_id`.

<a id="lesson-4"></a>
### ✍️ Lesson 4 – Prompts (`Lesson_4_Prompts`)
Focuses on **prompt engineering**: system messages, roles, instructions, and how prompt structure changes the model's answer. Same three flavors (OpenAI Chat, OpenAI Responses, Gemini).

<a id="lesson-5"></a>
### ⚙️ Lesson 5 – Model Settings (`Lesson_5_settings`)
Explores generation parameters: `temperature`, `top_p`, `max_tokens`, `frequency_penalty`, `presence_penalty`, stop sequences, etc. — and how they influence the output.

<a id="lesson-6"></a>
### 🌊 Lesson 6 – Streaming (`Lesson_6_streaming`)
Streaming responses token-by-token using `IAsyncEnumerable` / `await foreach`.
- `OpenAI_SDK_Response.cs` – OpenAI streaming
- `Gemini_SDK.cs` – Gemini streaming
- `YieldExample.cs` – a standalone demo of C# `yield return` / async streams as a primer for the streaming pattern.

<a id="lesson-7"></a>
### 🧱 Lesson 7 – Structured Output (JSON) (`Lesson_7_JSON`)
Forcing the model to return valid, typed **JSON** matching a C# schema.
- `Student.cs`, `Country.cs`, `CountriesResponse.cs` – target DTOs
- `JSON_example.cs` – plain JSON parsing helpers
- `OpenAI_example.cs` / `Gemini_example.cs` – using each SDK's structured-output / JSON-schema feature to deserialize straight into objects.

<a id="lesson-8"></a>
### 🛠️ Lesson 8 – Tools / Function Calling / ReAct Loop
Tool calling is split across several sub-projects, each one adding capability:

- **`Lesson_8_Tools`** – first tools: `DateTimeTools`, a manual **ReAct loop** (`AgentStep.cs`). Three progressive scenarios (`Tools1/2/3`) for both GPT and Gemini.
- **`Lesson_8_Tools2`** – adds **Tavily web search** (`TavilySearch.cs`) as a real tool.
- **`Lesson_8_Tools3`** – adds **SQL tools** (`SQLTools.cs`) querying the `AI_Programming.mdf` database, plus *thinking* variants (`Tools_GPT_Thinking`, `Tools_Gemini_Thinking`) that expose the model's reasoning steps.
- **`Lesson_8_Tools4`** – **server-side / built-in tools** hosted by the provider (`GPT_server_tools.cs`, `Gemini_server_tools.cs`) instead of the client orchestrating the loop.
- **`Lesson_8_Tools5`** – consolidated end-state with DateTime + Tavily + SQL tools and the thinking-mode models. Also enables a **server-side Python tool** — OpenAI's **Code Interpreter** and Gemini's **Code Execution** — letting the model run Python on the provider's sandbox for math, data crunching, and file analysis. Any **plots / charts** the Python code produces are downloaded from the sandbox and saved locally to the `plots/` folder so you can open them straight from the project.

<a id="lesson-9"></a>
### 🎨 Lesson 9 – Images & Voice
Multimodal generation.

- **`Lesson_9_images_Voice`** – first pass:
  - `OpenAI_Img.cs` – DALL·E / `gpt-image` image generation
  - `OpenAI_Voice.cs` – OpenAI **TTS** (text-to-speech)
  - `Google_img.cs` / `Google_Voice.cs` – equivalents on Gemini
- **`Lesson_9_images_Voice_2`** – refined version focused on **image create & edit** workflows, split into separate files:
  - `GPT_Create_Image.cs`, `GPT_Edit_Image.cs`
  - `Gemini_Create_Image.cs`, `Gemini_Edit_Image.cs`, `Gemini_Images.cs`
  - `OpenAI_Tools.cs` – generic wrapper that saves images when produced.

<a id="lesson-10"></a>
### 🧮 Lesson 10 – Embeddings (`Lesson_10_Embedding`)
Generating **text embeddings** with `OpenAI_Embeddings.cs`, computing cosine similarity, and exploring how semantically similar texts cluster in vector space.

<a id="lesson-11"></a>
### 📚 Lesson 11 – RAG
**Retrieval-Augmented Generation** over PDF documents.

- **`Lesson_11_RAG`** – client-side RAG:
  - `PdfLoader.cs` – chunk PDFs with UglyToad.PdfPig
  - `OpenAI_Embeddings.cs` – embed chunks
  - `PineconeClient.cs` – upsert & query a Pinecone index
  - `RagChat.cs` + `OpenAI_Tools.cs` – chat loop that retrieves top-k chunks and feeds them to the model
- **`Lesson_11_RAG_srv`** – server-side RAG using the **OpenAI file-search / vector store** feature: the PDF is uploaded, a vector store is created, and retrieval happens on OpenAI's side — no Pinecone needed.

<a id="lesson-12"></a>
### 🌐 Lesson 12 – Razor Pages UI (`Lesson_12_Razor_Pages`)
Wraps the previous concepts in an **ASP.NET Core Razor Pages** web app with session state (`AddSession`, `UseSession`) so the chat history survives across requests.

<a id="winforms"></a>
### 🖥️ WinForm Projects
Desktop front-ends that reuse the same SDK wrappers from the lessons:

- **`WinForm_1`** – basic chat form with OpenAI Chat + Responses + Gemini backends
- **`WinForm_2`** – Gemini-only chat UI
- **`WinForm_3`** – OpenAI Responses-based chat
- **`WinForm_4`** – combined OpenAI + Gemini chat UI

---

<a id="license"></a>
## 📄 License

Educational material by **Gilad Markman**. Use it, learn from it, break it, fix it. 🎓


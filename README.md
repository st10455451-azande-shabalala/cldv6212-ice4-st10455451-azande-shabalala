# CoffeeNChill — Cloud-Based Canteen Management System

**Module:** CLDV6212 — Cloud Development B
**Assessment:** Portfolio of Evidence (POE) — Part 1
**Student Numbers:** ST10479155 - Sonali Arjun | ST10454454 - Orlando Pillay | ST10455451 - Azande Shabalala

CoffeeNChill is a cloud-enabled microservices system built for a campus canteen, replacing paper menus and filing-cabinet documents with Azure Storage, serverless Azure Functions, and Docker containerization.

This repository covers **Part 1**: Azure Table Storage for the digital menu, Azure Blob Storage for staff documents, and standalone Dockerized Azure Functions running against a local Azurite emulator.
---

## Tech Stack

- **.NET 8** — Azure Functions Isolated Worker runtime
- **Azure.Data.Tables** SDK — Azure Table Storage access
- **Azure.Storage.Blobs** SDK — Azure Blob Storage access
- **Azurite** — local Azure Storage emulator (Blob, Queue, Table)
- **Docker** — containerization of Azurite and the Functions app
- **Postman** — API testing and documentation

---

## Project Structure
CoffeeNChill.Functions/
├── Functions/
│ ├── CreateMenuItem.cs # POST /api/menu [Sonali]
│ ├── GetAllMenuItems.cs # GET /api/menu [Sonali]
│ ├── GetMenuItemsByCategory.cs # GET /api/menu/category/{category} [Sonali]
│ ├── UpdateMenuItem.cs # PUT /api/menu/{category}/{id} [Sonali]
│ ├── DeleteMenuItem.cs # DELETE /api/menu/{category}/{id} [Sonali]
│ ├── UploadStaffDocument.cs # POST /api/documents/upload [Orlando]
│ ├── ListStaffDocuments.cs # GET /api/documents [Orlando]
│ └── DownloadStaffDocument.cs # GET /api/documents/download/{fileName} [Orlando]
├── Models/
│ ├── MenuItemEntity.cs [Sonali]
│ ├── MenuItemDto.cs [Sonali]
│ └── StaffDocumentInfo.cs [Orlando]
├── Services/
│ ├── TableStorageService.cs [Sonali]
│ └── BlobStorageService.cs [Orlando, fixed by Sonali]
├── Program.cs
├── Dockerfile [Orlando]
├── local.settings.json.example
├── .gitignore
└── host.json
Docs/
└── CoffeeNChill - Part 1.postman_collection.json [Azande]
.gitignore (repo root — excludes Azurite runtime files)

---

## Local Setup

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Functions Core Tools v4](https://go.microsoft.com/fwlink/?linkid=2174087)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Postman](https://www.postman.com/downloads/)

### 1. Clone the repository
```bash
git clone https://github.com/EMGPMD/cldv6212-group-1-poe-part-1-st10479155-sonali-arjun.git
```
Clone directly with `git clone` — do not download and extract a ZIP into an existing folder of the same name, as this creates a nested folder structure that can cause path-length build errors on Windows.

### 2. Configure local settings
Copy the example settings file:
```bash
copy local.settings.json.example local.settings.json
```
Confirm it contains:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```
This same connection string covers Table, Blob, and Queue emulation in Azurite — no separate configuration is needed for Blob Storage.

### 3. Start Azurite (standalone container)
Run this from a folder **outside** the cloned repository (e.g. your home directory), so Azurite's local runtime files are not created inside the project folder:
```bash
docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```
Leave this running in its own terminal window.

### 4. Run the Functions app locally
```bash
dotnet build
func start
```
Or press **F5** in Visual Studio.

Per `launchSettings.json`, the app runs on **port 7081** locally. The console will list all available endpoints:
Functions:
CreateMenuItem: [POST] http://localhost:7081/api/menu
GetAllMenuItems: [GET] http://localhost:7081/api/menu
GetMenuItemsByCategory: [GET] http://localhost:7081/api/menu/category/{category}
UpdateMenuItem: [PUT] http://localhost:7081/api/menu/{category}/{id}
DeleteMenuItem: [DELETE] http://localhost:7081/api/menu/{category}/{id}
UploadStaffDocument: [POST] http://localhost:7081/api/documents/upload
ListStaffDocuments: [GET] http://localhost:7081/api/documents
DownloadStaffDocument: [GET] http://localhost:7081/api/documents/download/{fileName}

### 5. Standalone Docker execution (Functions container)
```bash
docker build -t <dockerhub_username>/coffeenchill-functions:v1.0 .
docker run -p 7071:80 -e AzureWebJobsStorage="DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://host.docker.internal:10000/devstoreaccount1;QueueEndpoint=http://host.docker.internal:10001/devstoreaccount1;TableEndpoint=http://host.docker.internal:10002/devstoreaccount1;" <dockerhub_username>/coffeenchill-functions:v1.0
```

> **Important:** the Functions container must be run with `AzureWebJobsStorage` passed explicitly, pointing at `host.docker.internal` (Docker's DNS name for the host machine), rather than `UseDevelopmentStorage=true` — that value only resolves correctly for local (non-containerized) development. Running the container without a storage connection string results in a `500 Internal Server Error` on every endpoint, since the Functions host cannot reach any storage account. `host.docker.internal` lets the container reach the Azurite instance running on the host machine's ports 10000–10002.

### 6. Standalone Docker execution (Azurite container, published image)
```bash
docker pull st10455451/coffeenchill-azurite:v1.0
docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 st10455451/coffeenchill-azurite:v1.0
```

---

## API Endpoints

### Menu Management
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/menu` | Create a new menu item |
| GET | `/api/menu` | Get all menu items |
| GET | `/api/menu/category/{category}` | Get menu items filtered by category |
| PUT | `/api/menu/{category}/{id}` | Update a menu item's price/availability |
| DELETE | `/api/menu/{category}/{id}` | Delete a menu item |

**MenuItems Table Schema**
| Field | Type | Notes |
|-------|------|-------|
| PartitionKey | string | Category (e.g. "Hot Drinks") |
| RowKey | string | Unique SKU/ID (e.g. "COF-001") |
| Name | string | Item name |
| Description | string | Short menu description |
| Price | double | Item price |
| IsAvailable | bool | Availability status |

### Staff Documents (Azure Blob Storage)
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/documents/upload` | Uploads a file via multipart/form-data to the `staff-docs` Blob Storage container |
| GET | `/api/documents` | Lists all files stored in the `staff-docs` container, with name, size, and last modified date |
| GET | `/api/documents/download/{fileName}` | Streams the requested file back, or returns 404 if not found |

Allowed file types on upload: `.pdf`, `.png`, `.jpg`, `.jpeg`, `.doc`, `.docx`.

---

## Testing

A full Postman collection is available at `Docs/CoffeeNChill - Part 1.postman_collection.json`, covering all 5 menu endpoints and 3 document endpoints with positive and negative test cases (invalid input, not-found scenarios).

Import it into Postman, then:
1. Set the `baseUrl` collection variable to match how you're running the app — `http://localhost:7081/api` for local `func start`, or `http://localhost:7071/api` if testing against the Docker container.
2. For the "Upload Staff Document - valid" request, attach a real file of your own (PDF, PNG, JPEG, DOC, or DOCX) under the `file` form-data field, then update the `testFileName` collection variable to match the exact filename you attached, so the download tests can reference it.

---

## Docker Hub

- Functions image: `docker pull <dockerhub_username>/coffeenchill-functions:v1.0`
- Azurite image: `docker pull st10455451/coffeenchill-azurite:v1.0`

---

## Team Contributions

| Member | Student Number | Contribution |
|--------|----------------|---------------|
| Sonali Arjun | ST10479155 | Azurite local setup, `MenuItems` table design, all 5 Menu HTTP Functions (Create, GetAll, GetByCategory, Update, Delete), input validation and error handling, initial Postman collection and README drafts. Diagnosed and fixed Blob Storage integration bugs (incorrect container name, broken multipart/form-data upload handling, missing file validation) found while completing local functions testing and Docker container verification. Identified and documented the Docker container's storage connection-string requirement. |
| Orlando Pillay | ST10454454 | `staff-docs` Blob Storage integration (migrated from File Storage per addendum), Upload/List/Download document functions, Dockerfile, Docker Hub image publishing (Functions image), video recording and editing. |
| Azande Shabalala | ST10455451 | Azurite Docker Hub image publishing (`coffeenchill-azurite:v1.0`), built and maintained the merged Postman collection covering all menu and document endpoints, local Postman test verification. |

---

## Video Demonstration

https://youtu.be/TVw13rk1I38




# Progressa

🧠 **Progressa** is an intelligent platform that helps you build good habits, stay motivated, and make daily progress.

> Forget boring trackers. Discover a personalized development journey powered by AI that understands you.

## 🚀 Key Features
- ✅ Daily habit tracker with progress visualization
- 🧠 AI-generated challenges based on your goals and pace
- 🔔 Smart reminders and motivational messages
- 🏆 Achievements and badges to keep you going
- 👥 Group challenges with friends or family
- 📊 Analytics + AI-based suggestions to maintain momentum

## 🧱 Tech Architecture
- **Frontend:** Vue.js
- **Backend:** ASP.NET Core (.NET 8)
- **Database:** PostgreSQL
- **AI Modules:** C# + OpenAI
- **CI/CD:** GitHub Actions + Railway
- **Hosting:** Railway
- **Monorepo:** Yes

## 📁 Project Structure
- **apps/web-client** — Frontend SPA using Nuxt 3
- **apps/api-server** — Backend REST API using ASP.NET Core
- **libs/** — Shared libraries (AI, DTOs)
- **infrastructure/** — Docker, CI/CD, Railway, Helm configs
- **tests/** — Unit & integration tests
- **docs/** — Documentation and diagrams

## 🛠️ Getting Started

### Frontend
```bash
cd apps/web-client
npm install
npm run dev

### Backend
cd apps/api-server
dotnet restore
dotnet run

## 🐳 Running with Docker
docker-compose up --build

## 🧪 Testing

# API tests
cd tests/api-server
dotnet test

# Frontend tests
cd apps/web-client
npm run test

## 📦 Technologies Used
- Vue.js
- .NET 8 Web API
- PostgreSQL
- OpenAI (for intelligent challenge generation)
- GitHub Actions for CI/CD
- Docker & Railway for deployment

# MyCyberQuiz

MyCyberQuiz är en modern och interaktiv webbapplikation där användare kan registrera sig, svara på quiz, följa sina framsteg och få hjälp via en inbyggd AI-chatt. Applikationen är byggd med en fullstack .NET-arkitektur med **Blazor** som frontend och **ASP.NET Core Web API** som backend.

## 🏗️ Projektstruktur

Lösningen är uppdelad i flera projekt för att hålla en tydlig separation (N-Tier-arkitektur):

* **MyCyberQuiz (UI)**
    Presentationslagret byggt i **Blazor**. Här finns alla användargränssnittskomponenter, sidor (t.ex. QuizPlayer, Profil, Login) och tjänster som anropar backend-API:et.
* **MyCyberQuiz.API**
    Applikationens backend. Ett ASP.NET Core Web API som tar emot HTTP-förfrågningar från frontend och dirigerar dem till affärslogiken. Innehåller controllers för autentisering, quiz-hantering, profil och AI-chatt.
* **MyCyberQuiz.BLL (Business Logic Layer)**
    Affärslogiklagret. Hanterar reglerna för applikationen, inklusive quiz-logik, poängberäkning, JWT-autentisering och integration med AI-tjänster.
* **MyCyberQuiz.DAL (Data Access Layer)**
    Dataåtkomstlagret. Ansvarar för databasstrukturen och kommunikationen via **Entity Framework Core**. Här definieras alla databasmodeller (användare, quiz, frågor, svar och framsteg) samt en förkonfigurerad `AppDbContext` med seed-data.
* **MyCyberQuiz.Shared**
    Ett delat projekt som innehåller DTO:er (Data Transfer Objects). Eftersom både Blazor (UI) och API:et är skrivna i C# kan de dela dessa modeller, vilket minimerar kodduplicering och fel vid dataöverföring.

## ✨ Funktioner

* **Användarkonton & Säkerhet:** Registrering, inloggning och säker autentisering via JWT (JSON Web Tokens).
* **Interaktiva Quiz:** Bläddra bland kategorier och spela quiz med flervalsfrågor.
* **Framsteg & Poäng:** Sparar användarens poäng och spårar vilka quiz som är avklarade.
* **AI-Chatt:** En inbyggd AI-assistent som användare kan chatta med för att få hjälp eller lära sig mer.

## 🛠️ Teknisk stack

* **Språk:** C#
* **Frontend:** .NET Blazor (HTML, CSS, Bootstrap)
* **Backend:** ASP.NET Core Web API
* **Databas:** Microsoft SQL Server
* **ORM:** Entity Framework Core

## 🚀 Komma igång

För att köra projektet lokalt behöver du [.NET SDK](https://dotnet.microsoft.com/download) installerat.

1. **Klona projektet** till din lokala maskin.
2. **Öppna terminalen** i projektets rotmapp (eller öppna `MyCyberQuiz.UI.slnx` i Visual Studio / Rider).
3. **Uppdatera databasen**:
   Kör Entity Framework-migreringarna för att skapa databasen och fylla den med startdata (seed data). Kontrollera att anslutningssträngen i `MyCyberQuiz.API/appsettings.json` stämmer överens med din lokala SQL Server.
   ```bash
   dotnet ef database update --project MyCyberQuiz.DAL --startup-project MyCyberQuiz.API
Gemini sa
Här är ett förslag på en README.md för ditt nya projekt, MyCyberQuiz. Det här projektet är lite större och mer avancerat eftersom det använder en Blazor-frontend (UI) som pratar med ett separat API, och har spännande funktioner som AI-chatt!

Du kan spara koden nedan som README.md i rotmappen av lösningen:

Markdown
# MyCyberQuiz

MyCyberQuiz är en modern och interaktiv webbapplikation där användare kan registrera sig, svara på quiz, följa sina framsteg och få hjälp via en inbyggd AI-chatt. Applikationen är byggd med en fullstack .NET-arkitektur med **Blazor** som frontend och **ASP.NET Core Web API** som backend.

## 🏗️ Projektstruktur

Lösningen är uppdelad i flera projekt för att hålla en tydlig separation (N-Tier-arkitektur):

* **MyCyberQuiz (UI)**
    Presentationslagret byggt i **Blazor**. Här finns alla användargränssnittskomponenter, sidor (t.ex. QuizPlayer, Profil, Login) och tjänster som anropar backend-API:et.
* **MyCyberQuiz.API**
    Applikationens backend. Ett ASP.NET Core Web API som tar emot HTTP-förfrågningar från frontend och dirigerar dem till affärslogiken. Innehåller controllers för autentisering, quiz-hantering, profil och AI-chatt.
* **MyCyberQuiz.BLL (Business Logic Layer)**
    Affärslogiklagret. Hanterar reglerna för applikationen, inklusive quiz-logik, poängberäkning, JWT-autentisering och integration med AI-tjänster.
* **MyCyberQuiz.DAL (Data Access Layer)**
    Dataåtkomstlagret. Ansvarar för databasstrukturen och kommunikationen via **Entity Framework Core**. Här definieras alla databasmodeller (användare, quiz, frågor, svar och framsteg) samt en förkonfigurerad `AppDbContext` med seed-data.
* **MyCyberQuiz.Shared**
    Ett delat projekt som innehåller DTO:er (Data Transfer Objects). Eftersom både Blazor (UI) och API:et är skrivna i C# kan de dela dessa modeller, vilket minimerar kodduplicering och fel vid dataöverföring.

## ✨ Funktioner

* **Användarkonton & Säkerhet:** Registrering, inloggning och säker autentisering via JWT (JSON Web Tokens).
* **Interaktiva Quiz:** Bläddra bland kategorier och spela quiz med flervalsfrågor.
* **Framsteg & Poäng:** Sparar användarens poäng och spårar vilka quiz som är avklarade.
* **AI-Chatt:** En inbyggd AI-assistent som användare kan chatta med för att få hjälp eller lära sig mer.

## 🛠️ Teknisk stack

* **Språk:** C#
* **Frontend:** .NET Blazor (HTML, CSS, Bootstrap)
* **Backend:** ASP.NET Core Web API
* **Databas:** Microsoft SQL Server
* **ORM:** Entity Framework Core

## 🚀 Komma igång
Det finns en seedad user från start.
Email: user@example.com
Password: Password1234!

För att köra projektet lokalt behöver du [.NET SDK](https://dotnet.microsoft.com/download) installerat.

1. **Klona projektet** till din lokala maskin.
2. **Öppna terminalen** i projektets rotmapp (eller öppna `MyCyberQuiz.UI.slnx` i Visual Studio / Rider).
3. **Uppdatera databasen**:
   Kör Entity Framework-migreringarna för att skapa databasen och fylla den med startdata (seed data). Kontrollera att anslutningssträngen i `MyCyberQuiz.API/appsettings.json` stämmer överens med din lokala SQL Server.
   ```bash
   dotnet ef database update --project MyCyberQuiz.DAL --startup-project MyCyberQuiz.API
4. Kör applikationen:
   Eftersom detta projekt har ett separat API och ett UI måste båda vara igång samtidigt.
   Om du kör projektet i Visual Studio kan du ställa in **"Multiple Startup Projects"** genom att högerklicka på din *Solution*
   -> *Properties* -> *Startup Project* och välja att både `MyCyberQuiz` (UI) och `MyCyberQuiz.API` ska starta när du trycker på "Play".

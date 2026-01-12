# Weather App 🌦️

An application built with **ASP.NET Core (C#)** that displays current weather conditions and forecasts for Romanian cities using **official Romanian meteorological data (ANM)**.

The project focuses on **clean architecture** and **software testing techniques**, including **boundary testing, stubs, and mocks**, as part of an academic assignment.

---

## 🎯 Project Goals

- Consume real Romanian weather data using public ANM endpoints (JSON & XML)
- Apply **unit testing strategies** 
- Demonstrate proper separation of concerns and dependency injection

---

## 🧠 Features

- 🌍 Current weather by city (temperature, humidity, wind, pressure)
- 📄 5-day forecast parsed from XML data
- 🔌 External API integration (ANM – Meteo Romania)
- 🧪 Comprehensive unit testing:
  - Stubbed weather API responses
  - Mocked service dependencies
  - Boundary condition testing
- ⚙️ RESTful backend built with ASP.NET Core Web API

---

## 🏗️ Project Architecture

weather-app/

│

├── backend/

│ ├── RomanianWeather.API/

│ │ ├── Controllers/

│ │ ├── Services/

│ │ ├── Providers/

│ │ ├── Interfaces/

│ │ ├── Models/

│ │ └── Program.cs

│ │

│ └── RomanianWeather.Tests/

│ ├── ServiceTests/

│ ├── ControllerTests/

│ ├── Stubs/

│ └── Mocks/

│

└── README.md

---

## 🔗 Data Sources (APIs)

- **Current Weather (JSON)**  
  ANM public endpoint via `meteoromania.ro`

- **5-Day Forecast (XML)**  
  https://www.meteoromania.ro/anm/prognoza-orase-xml.php

> Although some endpoints return downloadable XML files, they are treated as APIs and parsed.

---

## 🧪 Testing Strategy

This project applies multiple testing techniques:

### ✔ Boundary Testing
- Empty or invalid city names
- Extreme temperature values
- Missing or malformed API fields
- Null or failed API responses

### ✔ Stub Testing
- Weather API calls are replaced with **stub implementations**
- Allows testing without internet access
- Ensures predictable and repeatable test results

### ✔ Mock Testing
- Uses mocking frameworks (e.g., Moq)
- Verifies interactions between controllers and services
- Confirms correct behavior and method calls
- 
---

## 🚀 Technologies Used

- C# / .NET (ASP.NET Core Web API)
- REST APIs
- XML & JSON parsing
- Dependency Injection
- MSTest / NUnit
- Moq

---

## 📚 Academic Context

This project was developed as part of a **software testing and quality assurance course**.

---

## 👤 Author

**Alin Lemnaru**  
4th-year Systems Engineering student  
Passionate about software architecture, testing, and backend development

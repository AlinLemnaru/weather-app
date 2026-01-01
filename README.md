# Weather App 🌦️

A web application built with **ASP.NET Core (C#)** that displays current weather conditions and forecasts for Romanian cities using **official Romanian meteorological data (ANM)**.

The project focuses on **clean architecture** and **software testing techniques**, including **boundary testing, stubs, and mocks**, as part of an academic assignment.

---

## 🎯 Project Goals

- Consume real Romanian weather data using public ANM endpoints (JSON & XML)
- Provide a simple web-based UI for displaying weather information
- Apply **unit testing strategies** used in real-world software engineering
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
- 🌐 Simple web frontend (HTML / CSS / JavaScript)
- ⚙️ RESTful backend built with ASP.NET Core Web API

---

## 🏗️ Project Architecture

RomanianWeatherWebApp/
│
├── backend/
│ ├── RomanianWeather.API/
│ │ ├── Controllers/
│ │ ├── Services/
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
├── frontend/
│ ├── index.html
│ ├── styles.css
│ └── app.js
│
└── README.md

---

## 🔗 Data Sources (APIs)

- **Current Weather (JSON)**  
  ANM public endpoint via `meteoromania.ro`

- **5-Day Forecast (XML)**  
  https://www.meteoromania.ro/anm/prognoza-orase-xml.php

> Although some endpoints return downloadable XML files, they are treated as APIs and parsed programmatically.

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

---

## 🖥️ Frontend

- Simple, lightweight web interface
- Communicates with backend via REST API
- Focus kept on backend logic and testing

---

## 🚀 Technologies Used

- C# / .NET (ASP.NET Core Web API)
- REST APIs
- XML & JSON parsing
- Dependency Injection
- MSTest / NUnit
- Moq
- HTML / CSS / JavaScript

---

## 📚 Academic Context

This project was developed as part of a **software testing and quality assurance course**, extending previous work by applying the same testing strategies to a **more complex, real-world system**.

---

## 👤 Author

**Alin Lemnaru**  
3rd-year Systems Engineering student  
Passionate about software architecture, testing, and backend development

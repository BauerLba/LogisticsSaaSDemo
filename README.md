# LogiFlow – SaaS Logistics Platform

LogiFlow is a state-of-the-art SaaS logistics management application built with **Blazor Web App (.NET 8)**. It features a stunning, premium design with a focus on real-time visibility, clean architecture, and highly interactive user experiences.

![LogiFlow Dashboard](https://images.unsplash.com/photo-1586528116311-ad8dd3c8310d?auto=format&fit=crop&q=80&w=2000)

## ✨ Key Features

- **🚀 Real-Time Dashboard**: Dynamic overview of logistics metrics, active shipments, and live filtering of operational data.
- **📦 Shipment Management**: Comprehensive management of the global supply chain, including detailed views of every package.
- **🔍 Advanced Tracking**: Interactive "Track Your Assets" page with real-time status timelines, milestones, and progress tracking.
- **🤝 Customer Network**: Partner and client relationship management with distinct profiles and performance metrics.
- **📈 AI-Driven Analytics**: Premium reports placeholder featuring AI-themed insights and route efficiency analysis.
- **🌗 Modern Aesthetics**: A sleek "Glassmorphism" design system using Vanilla CSS, dark mode, rich icons (FontAwesome), and smooth micro-animations.
- **⚡ Global Search**: Instant access to any shipment directly from the header via tracking number.

## 🏗️ Technical Architecture

The project follows the principles of **Clean Architecture**, ensuring a decoupled and maintainable codebase:

- **Core.Domain**: Business entities (Shipment, Customer), enums, and core logic.
- **Core.Application**: Interfaces and services (ShipmentService, CustomerService) that coordinate application tasks.
- **Infrastructure**: Data access implementations (In-memory repositories for demo purposes).
- **Web**: Blazor frontend using **Interactive Server** render mode for high reactivity.

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 8.0 (Blazor Web App)
- **Language**: C# 12
- **Styling**: Vanilla CSS (Custom Glassmorphism System)
- **Icons**: FontAwesome 6.4
- **Fonts**: Google Fonts (Inter, Outfit)
- **Components**: Interactive Razor Components

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022, JetBrains Rider, or VS Code

### Installation & Run

1. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/LogisticsSaaS.git
   cd LogisticsSaaS
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Run the application**:
   ```bash
   dotnet run --project LogisticsSaaS.Web
   ```

4. **Navigate to**: `http://localhost:5140`

## 🧪 Demo Data

The application comes pre-loaded with sample data for demonstration:
- **Tracking Numbers**: `TRK-DEMO-123`, `TRK-DEMO-456`, `TRK-DEMO-789`
- **Searchable Locations**: Berlin, Paris, Hamburg, Munich, London.

---

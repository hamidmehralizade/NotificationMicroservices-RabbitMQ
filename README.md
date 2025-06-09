
# Notification Microservice with RabbitMQ & .NET 8

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.12-orange)
![Docker](https://img.shields.io/badge/Docker-24.0-blue)
![YARP](https://img.shields.io/badge/YARP-1.2-green)

A high-performance notification system using microservices architecture with RabbitMQ message broker and .NET 8.

## 🌟 Key Features
- **Multi-channel Notifications**: Email, SMS, Push
- **Async Processing**: RabbitMQ-backed message queue
- **Scalable Workers**: BackgroundService-based consumers
- **Reverse Proxy**: YARP for API gateway
- **Health Monitoring**: Integrated health checks
- **Containerized**: Ready for Docker deployment

## 🛠 Tech Stack
| Component | Technology |
|--------------------|--------------------------|
| API Gateway | YARP + .NET 8 Minimal API|
| Message Broker | RabbitMQ 3.12 |
| Workers | .NET 8 BackgroundService |
| Containerization | Docker Compose |
| CI/CD | GitHub Actions | 
## 📦 Architecture
```mermaid
graph LR
    Client -->|HTTP| APIGateway(YARP Gateway)
    APIGateway -->|ReverseProxy| NotificationService
    NotificationService -->|Publish| RabbitMQ
    RabbitMQ -->|Consume| EmailWorker
    RabbitMQ -->|Consume| SmsWorker
    RabbitMQ -->|Consume| PushWorker
 ```
## 🚀 Quick Start
### Clone and run
git clone https://github.com/hamidmehralizade/NotificationMicroservices-RabbitMQ.git

cd src

docker-compose up -d --build

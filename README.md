# customer_management_system
Customer Management System is a lightweight, containerized .NET Core API for managing customer data. It exposes RESTful endpoints to create, update, and retrieve customer information. The project is designed for easy deployment using Docker and Kubernetes, making it suitable for scalable and cloud-native environments.

# how_to_run

🚀 Features
- Built with ASP.NET Core Web API
- Dockerized for containerized deployment
- Kubernetes manifests for orchestration
- Shell scripts for easy local development
- Environment-specific configuration with `.env` support

🔐 Create .env File
Before running the application, you must create a `.env` file in the root directory. This file will contain your MongoDB credentials and must not be committed to version control.
Example `.env` file content:
MONGO_USER=<your_admin_username>
MONGO_PASSWORD=<your_admin_password>


📁 Project Structure
customer_management/
├── CustomerApi/           # Main .NET API project
├── docker-compose.yml     # Docker Compose config
├── dotnet.sln             # .NET solution file
├── .env                   # Environment variables
├── k8s/                   # Kubernetes manifests
├── start.sh               # Shell script to start the app
├── stop.sh                # Shell script to stop the app
└── logs.sh                # Shell script to view logs

🛠️ Prerequisites
Make sure the following are installed on your machine:
- .NET SDK 7.0+: https://dotnet.microsoft.com/en-us/download
- Docker: https://www.docker.com/
- Docker Compose: https://docs.docker.com/compose/
- (Optional) kubectl and a running Kubernetes cluster (e.g., Minikube or kind)

💻 Running Locally
🔧 Using .NET CLI
cd customer_management
dotnet build dotnet.sln
dotnet run 

🧪 Testing
You can use Postman for your tests, with that address:
https://localhost:5001 or http://localhost:5000

The API should be also accessible at https://localhost:5001 or http://localhost:5000
🐳 Using Docker Compose
cd customer_management
docker-compose up --build

Access the API using the ports defined in docker-compose.yml
📜 Using Shell Scripts
chmod +x start.sh stop.sh logs.sh
./start.sh
./stop.sh
./logs.sh

☁️ Kubernetes Deployment
Ensure you have a Kubernetes cluster set up (e.g., with Minikube or kind).
kubectl apply -f k8s/

🔐 Environment Configuration
Environment variables can be set in the `.env` file located at the root of the project. These are automatically picked up by Docker and shell scripts.

🛑 Stopping the Application
To stop the running containers, use the following command:
docker-compose down

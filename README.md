# customer_management_system
Customer Management System is a lightweight, containerized .NET Core API for managing customer data. It exposes RESTful endpoints to create, update, and retrieve customer information. The project is designed for easy deployment using Docker and Kubernetes, making it suitable for scalable and cloud-native environments.

# how_to_run
Prerequisites
Ensure the following tools are installed on your machine:
•	- .NET 6+ SDK
•	- Docker
•	- Docker Compose
•	- (Optional) MongoDB Compass for database visualization

🔐 Create .env File
Before running the application, you must create a `.env` file in the root directory. This file will contain your MongoDB credentials and must not be committed to version control.
Example `.env` file content:
MONGO_USER=<your_admin_username>
MONGO_PASSWORD=<your_admin_password>

🐳 Running the Application with Docker Compose
1. Clone the repository:
git clone https://github.com/your-username/customer-management.git
cd customer-management
2. Build and start the containers:
docker-compose up --build
3. Access the API at:
http://localhost:8080
4. Access the MongoDB instance at:
mongodb://admin:supersecret@localhost:27017

🛑 Stopping the Application
To stop the running containers, use the following command:
docker-compose down

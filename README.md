# Project Overview

This project is intended for **Limited Input Device Authentication with Spotify**. The concept behind the code is to enable a limited input device to authenticate with Spotify by following these steps:

1. **Code Creation**: The limited input device uses the `/api/code` endpoint to create a code. This code is a randomly generated string that gets saved to the server's in-memory storage.

2. **Authorization Flow**: On another device (such as a mobile phone or computer), you navigate to the `/api/login` endpoint to perform Spotify's Authorization Code flow. You pass the generated code as a parameter, which then gets encrypted and sent as the `state` parameter to Spotify.

3. **Callback Handling**: When Spotify completes the authorization, it sends a callback to the server. The server then decrypts the `state` parameter to retrieve the original code and updates the corresponding entry in memory with the access token obtained during the token exchange process.

4. **Polling for Token**: While this process is ongoing, the limited input device is polling the server using the `/api/poll` endpoint to check if the token has been added as a value yet.

This design allows devices with limited input capabilities (e.g., smart speakers or IoT devices) to initiate and complete the Spotify authorization process by leveraging another device for user interaction.# Setting Up Spotify Credentials and Encryption Key using .NET User Secrets

To store your Spotify client ID, client secret, and encryption key securely during development, you can use the `.NET User Secrets` tool. This allows you to store sensitive information separately from your source code, ensuring it remains secure and isn't accidentally shared.

Here are the commands you need to run to set up your secrets:

### Initialize User Secrets

In your project directory, initialize user secrets by running:

```sh
cd your_project_directory

# Initialize user secrets in your project
 dotnet user-secrets init
```

This will create a reference to a secure storage location for your secrets.

### Step 2: Set Secrets for Spotify Credentials and Encryption Key

To store the client ID, client secret, and encryption key, run the following commands:

```sh
# Set Spotify Client ID
 dotnet user-secrets set "Spotify:ClientId" "your_client_id_here"

# Set Spotify Client Secret
 dotnet user-secrets set "Spotify:ClientSecret" "your_client_secret_here"

# Set Encryption Key
 dotnet user-secrets set "EncryptionKey" "your_encryption_key_here"
```

Replace `your_client_id_here`, `your_client_secret_here`, and `your_encryption_key_here` with your actual values.

### Step 3: Access Secrets in Your Code

Your `.NET` application can now access these secrets from `builder.Configuration` as follows:

```csharp
string clientId = builder.Configuration["Spotify:ClientId"];
string clientSecret = builder.Configuration["Spotify:ClientSecret"];
string encryptionKey = builder.Configuration["EncryptionKey"];
```

The secrets are securely stored and accessed, allowing your application to use them without exposing them in your source code.

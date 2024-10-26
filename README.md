# Setting Up Spotify Credentials and Encryption Key using .NET User Secrets

To store your Spotify client ID, client secret, and encryption key securely during development, you can use the `.NET User Secrets` tool. This allows you to store sensitive information separately from your source code, ensuring it remains secure and isn't accidentally shared.

Here are the commands you need to run to set up your secrets:

### Step 1: Initialize User Secrets

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

# RSA Secure Messaging System

## Overview

This RSA Secure Messaging System is a robust implementation of public key encryption for secure communication. Designed as a final project for the RIT CSCI-251 course for Fall 2023, it allows users to securely exchange messages.

## Features

- **RSA Encryption**: Utilizes RSA algorithm for secure message encryption and decryption.
- **Key Generation**: Generates RSA key pairs (public and private keys).
- **Message Encryption**: Encrypts messages using the recipient's public key.
- **Message Decryption**: Decrypts received messages using the user's private key.
- **Server Interaction**: Handles storing and retrieving public keys from a server.

## Getting Started

### Prerequisites

- .NET Core SDK
- Access to a server for key storage (configuration details provided separately).

### Installation

1. Clone the repository from GitHub.
2. Navigate to the project directory.

### Generating RSA Keys

To generate a new RSA key pair:

```sh
dotnet run keyGen <keysize>
```

Replace <keysize> with the desired key size (e.g., 1024, 2048).

### Sending and Retrieving Keys

To send your public key to the server:

```sh
dotnet run sendKey <your_email>
```

To retrieve another user's public key:

```sh
dotnet run getKey <user_email>
```

### Sending and Receiving Messages

To send a secure message:

```sh
dotnet run sendMsg <recipient_email> <"message">
```

To retrieve and decrypt messages sent to you:
```sh
dotnet run getMsg <your_email>
```

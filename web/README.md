# Stupid Chat

Angular 19 web application for the "Stupid Chat" test assignment.

## Description

This is a simple web application for working with chat APIs. The application allows you to:
- View a list of all chats
- Create new chats
- View chat content
- Ask questions in a chat (receive "I don't know" response)

## Getting Started

### Prerequisites

- Node.js (version 18.19.1, 20.11.1 or higher 22.0.0)
- npm or yarn

### Installing Dependencies

```bash
npm install
```

### Running in Development Mode

```bash
npm start
```

The application will be available at: http://localhost:4200

## API

The application works with .NET API at: https://localhost:5298/api/chats

### Endpoints

- `GET /api/chats/{id}` - get chat by ID
- `POST /api/chats/create` - create new chat
- `GET /api/chats/search?keyword=...` - search chats
- `POST /api/chats/{id}/ask` - ask question in chat

## Functionality

### Home Page
- Display list of all chats
- Search through chats
- Create new chat button

### Chat Creation
- Form for entering username
- Form validation
- Automatic navigation to created chat

### Chat View
- Display chat information
- List of all questions and answers
- Form for asking new questions
- Navigation back to chat list

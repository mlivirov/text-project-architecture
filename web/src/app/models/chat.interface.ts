export interface Chat {
  id: number;
  user: string;
  conversation: ChatConversation;
}

export type ChatConversation = { [key: string]: string }

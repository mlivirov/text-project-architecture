import {BehaviorSubject, map} from "rxjs";
import {Chat} from "./models/chat.interface";
import {ChatService} from "./services/chat.service";
import {Injectable} from "@angular/core";
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class AppState {
  constructor(
    private chatService: ChatService,
    private router: Router,
  ) {
  }

  public loading$ = new BehaviorSubject<boolean>(false);
  public askingQuestion$ = new BehaviorSubject<boolean>(false);
  public error$ = new BehaviorSubject<string | null>(null);
  public currentChat$ = new BehaviorSubject<Chat | null>(null);
  public chats$ = new BehaviorSubject<Chat[]>([]);

  public get getCurrentChat(): Chat | null {
    return this.currentChat$.value;
  }

  public currentConversation$ = new BehaviorSubject<any[]>([]);

  public updateConversation(chat: Chat) {
    const conversation = Object.entries(chat.conversation || {}).map(([q, a]) => ({question: q, answer: a}));
    this.currentConversation$.next(conversation);
  }

  public hasConversation$ = this.currentConversation$.pipe(map(conversation => conversation.length > 0));

  public initApp(): void {
    this.searchChats();
  }

  public searchChats(keyword?: string): void {
    this.error$.next(null);
    this.loading$.next(true);
    this.chatService.searchChats(keyword || '').subscribe({
      next: (chats) => {
        this.chats$.next(chats);
      },
      error: (error) => {
        this.error$.next(error.message);
      },
      complete: () => {
        this.loading$.next(false);
      }
    });
  }

  public initChat(id: number): void {
    this.currentChat$.next(null);
    this.loading$.next(true);
    this.chatService.getChat(id).subscribe({
      next: (chat) => {
        this.currentChat$.next(chat);
        this.updateConversation(chat);
      },
      error: (error) => {
        this.error$.next(error.message);
      },
      complete: () => {
        this.loading$.next(false);
      }
    });
  }

  public askQuestion(id: number, question: string): void {
    this.error$.next(null);
    this.askingQuestion$.next(true);
    this.chatService.askQuestion(id, question).subscribe({
      next: (answer: string) => {

        const withAnswer = {
          ...this.currentChat$.value,
          conversation: {
            ...this.currentChat$.value?.conversation,
            [question]: answer
          }
        }

        this.currentChat$.next(withAnswer as Chat);
        this.updateConversation(withAnswer as Chat);
        this.error$.next(null);
      },
      error: (error) => {
        this.error$.next(error.message);
      },
      complete: () => {
        this.askingQuestion$.next(false);
      }
    });
  }

  public createChat(user: string): void {
    const userName = user.trim();
    if (!userName) {
      this.error$.next('Please enter a username');
      return;
    }

    const newChat = {
      user: userName,
      conversation: {}
    };

    this.loading$.next(true);
    this.chatService.createChat(newChat).subscribe({
      next: (chatId) => {
        this.router.navigate(['/chat', chatId]);
      },
      error: (error) => {
        this.error$.next(error.message);
      },
      complete: () => {
        this.loading$.next(false);
      }
    });
  }
}

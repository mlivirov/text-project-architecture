import {Component, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {Router} from '@angular/router';
import {Chat} from '../../models/chat.interface';
import {AppState} from '../../app.state';

@Component({
  selector: 'app-chat-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [AppState],
  templateUrl: './chat-list.component.html',
  styleUrl: './chat-list.component.css'
})
export class ChatListComponent implements OnInit {
  public searchKeyword: string = '';

  public constructor(
    public state: AppState,
    private router: Router,
  ) {
  }

  public ngOnInit(): void {
    this.state.initApp();
  }

  public searchChats(): void {
    this.state.searchChats(this.searchKeyword);
  }

  public getChatConversations(chat: Chat): string[] {
    return Object.keys(chat.conversation)
  }

  public getChatMessagesCount(chat: Chat): number {
    return Object.keys(chat.conversation).length
  }


  public openChat(chatId: number): void {
    this.router.navigate(['/chat', chatId]);
  }

  public createNewChat(): void {
    this.router.navigate(['/create']);
  }
}

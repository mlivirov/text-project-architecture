import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import * as rxjs from 'rxjs';
import {Chat} from '../models/chat.interface';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private apiUrl = 'http://localhost:5298/api/chats';

  constructor(private http: HttpClient) {
  }

  public getChat(id: number): rxjs.Observable<Chat> {
    return this.http.get<Chat>(`${this.apiUrl}/${id}`).pipe(rxjs.delay(this.randomDelay()));
  }

  public createChat(chat: Omit<Chat, 'id'>): rxjs.Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/create`, chat).pipe(rxjs.delay(this.randomDelay()));
  }

  public searchChats(keyword: string): rxjs.Observable<Chat[]> {
    return this.http.get<Chat[]>(`${this.apiUrl}/search`, {
      params: {keyword}
    }).pipe(rxjs.delay(this.randomDelay()));
  }

  public askQuestion(chatId: number, question: string): rxjs.Observable<string> {
    return this.http.post(`${this.apiUrl}/${chatId}/ask`, {question}, {responseType: 'text'}).pipe(rxjs.delay(this.randomDelay()));
  }

  private randomDelay = () => {
    return Math.random() * 500 + 100;
  };
}

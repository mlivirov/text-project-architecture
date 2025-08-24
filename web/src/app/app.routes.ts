import {Routes} from '@angular/router';
import {ChatListComponent} from './components/chat-list/chat-list.component';
import {CreateChatComponent} from './components/create-chat/create-chat.component';
import {ChatViewComponent} from './components/chat-view/chat-view.component';

export const routes: Routes = [
  {path: '', component: ChatListComponent},
  {path: 'create', component: CreateChatComponent},
  {path: 'chat/:id', component: ChatViewComponent},
  {path: '**', redirectTo: ''}
];

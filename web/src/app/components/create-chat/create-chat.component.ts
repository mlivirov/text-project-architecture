import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {Router} from '@angular/router';
import {AppState} from '../../app.state';

@Component({
  selector: 'app-create-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-chat.component.html',
  styleUrl: './create-chat.component.css'
})
export class CreateChatComponent {
  public userName: string = '';

  public constructor(
    public state: AppState,
    private router: Router
  ) {
  }

  public goBack(): void {
    this.router.navigate(['/']);
  }
}

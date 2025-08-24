import {Component, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {Chat} from '../../models/chat.interface';
import {AppState} from '../../app.state';

@Component({
  selector: 'app-chat-view',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-view.component.html',
  styleUrl: './chat-view.component.css'
})
export class ChatViewComponent implements OnInit {
  public newQuestion: string = '';
  public id: number = 0;

  public constructor(
    private route: ActivatedRoute,
    private router: Router,
    public state: AppState
  ) {
  }

  public ngOnInit(): void {
    this.id = +this.route.snapshot.paramMap.get('id')!;
    this.state.initChat(this.id);
  }

  public get user(): string | null {
    return this.state.getCurrentChat?.user || null;
  }

  public askQuestion(): void {
    if (!this.newQuestion.trim()) {
      return;
    }

    this.state.askQuestion(this.id, this.newQuestion.trim());
    this.newQuestion = '';
  }

  public goBack(): void {
    this.router.navigate(['/']);
  }
}

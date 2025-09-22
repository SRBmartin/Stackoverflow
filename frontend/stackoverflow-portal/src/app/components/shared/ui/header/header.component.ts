import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BasicButtonComponent } from '../button/basic-button.component';
import { SearchInputComponent } from '../search-input/search-input.component';
import { QuestionService } from '../../../../core/services/question.service';
import { Router } from '@angular/router';
import { AskQuestionDialogComponent } from '../../../../modules/questions/create-question/ask-question-dialog.component';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: true,
  imports: [
    CommonModule, 
    BasicButtonComponent, 
    SearchInputComponent,
    AskQuestionDialogComponent
  ]
})
export class HeaderComponent {
  showAskDialog = false;

  constructor(private questionService: QuestionService, private router: Router) {}

  isLoggedIn(): boolean {
    return true;
  }

  onLogin() { 
  }

  onSignup() {
  }

  onSearch(query: string) {
  }

  onProfile() { 
  }

  onAskQuestion() { this.showAskDialog = true; }

  onDialogClosed() { this.showAskDialog = false; }

  onDialogSubmitted(data: { title: string; description: string; file?: File }) {
    this.questionService.createQuestion(data.title, data.description).subscribe({
      next: (q) => {
        const questionId = (q && ((q as any).id || (q as any).Id || (q as any).ID)) ?? (q && q?.Id) ?? null;
        const id = questionId ?? (q && q?.Id) ?? (q && q?.id);

        if (!id) {
          console.error('Ne mogu dobiti id iz odgovora:', q);
          this.showAskDialog = false;
          return;
        }

        if (data.file) {
          this.questionService.uploadQuestionPhoto(id, data.file).subscribe({
            next: () => this.router.navigate(['/questions', id]),
            error: (err) => {
              console.error('Upload photo failed', err);
              this.router.navigate(['/questions', id]);
            }
          });
        } else {
          this.router.navigate(['/questions', id]);
        }

        this.showAskDialog = false;
      },
      error: (err) => {
        console.error('Create question failed', err);
        this.showAskDialog = false;
      }
    });
  }
}


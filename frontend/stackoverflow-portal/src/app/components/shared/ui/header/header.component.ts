import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BasicButtonComponent } from '../button/basic-button.component';
import { SearchInputComponent } from '../search-input/search-input.component';
import { QuestionService } from '../../../../core/services/question.service';
import { Router } from '@angular/router';
import { AskQuestionDialogComponent } from '../../../../modules/questions/create-question/ask-question-dialog.component';
import { ToastServer } from '../../../../common/ui/toast/toast.service';
import { LoaderService } from '../../../../common/ui/loader/loader.service';
import { SearchService } from '../../../../core/services/shared/search.service';

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
  constructor(private searchService: SearchService) {}

  showAskDialog = false;

  constructor(
    private questionService: QuestionService,
    private router: Router, 
    private toast: ToastServer, 
    private loader: LoaderService
  ) {}

  isLoggedIn(): boolean {
    return true;
  }

  onLogin() { }
  onSignup() { }
  onSearch(query: string) {
    this.searchService.setSearchTerm(query);
  }
  onProfile() { }

  onAskQuestion() { 
    this.showAskDialog = true; 
  }

  onDialogClosed() { 
    this.showAskDialog = false; 
  }

  onDialogSubmitted(data: { title: string; description: string; file?: File }) {
    this.loader.show();
    this.showAskDialog = false; 
    
    this.questionService.createQuestion(data.title, data.description).subscribe({
      next: (q) => {
        const questionId = q?.id ?? q?.Id ?? q?.ID;
        if (!questionId) {

          this.toast.showToast('Failed to get question ID', 'error');
          this.loader.hide();
          return;
        }

        const navigateToQuestion = () => {
          this.router.routeReuseStrategy.shouldReuseRoute = () => false;
          this.router.onSameUrlNavigation = 'reload';
        
          this.router.navigate(['/questions', questionId]).then(() => {
            this.router.routeReuseStrategy.shouldReuseRoute = () => true;
          });
        };
        

        if (data.file) {
          this.questionService.uploadQuestionPhoto(questionId, data.file).subscribe({
            next: () => {
              this.toast.showToast('Your question has been posted successfully', 'success');
              navigateToQuestion();
            },
            error: () => {
              this.toast.showToast('Photo upload failed, but question was created', 'warning');
              navigateToQuestion();
            },
            complete: () => {
              this.loader.hide();
            }
          });
        } else {
          this.toast.showToast('Your question has been posted successfully', 'success');
          navigateToQuestion();
          this.loader.hide();
        }
      },
      error: (err) => {
        let errorMessage = err?.error?.message ||  'Failed to post your question';
        this.toast.showToast(errorMessage, 'error');
        this.loader.hide();
      }
    });
  }
}

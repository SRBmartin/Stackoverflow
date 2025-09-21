import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { QuestionResponseDto, QuestionDto } from '../../../core/dto/questions/question-response-dto';
import { AuthService } from '../../../core/auth/services/auth.service';
import { QuestionService } from '../../../core/services/question.service';
import { FormsModule } from '@angular/forms';
import { LoaderService } from '../../../common/ui/loader/loader.service';
import { ToastServer } from '../../../common/ui/toast/toast.service';


type VoteValue = '+' | '-' | null;

@Component({
  selector: 'app-question-details',
  standalone: true,
  imports: [CommonModule, HttpClientModule, FormsModule],
  templateUrl: './question-details.component.html',
  styleUrls: ['./question-details.component.scss']
})
export class QuestionDetailsComponent implements OnInit {
  question: QuestionDto | null = null; // jedno pitanje
  loading = true;
  originalPhotoUrl: SafeUrl | null = null;

  questionPhotoUrl: SafeUrl | null = null;
  userPhotoUrl: SafeUrl | null = null;

  editMode = false;
  editedTitle = '';
  editedDescription = '';

  editedPhotoFile: File | null = null;
  editedPhotoUrl: SafeUrl | null = null;

  answerUserPhotos: { [userId: string]: SafeUrl | null } = {};

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private sanitizer: DomSanitizer, 
    private authService: AuthService, 
    private questionService: QuestionService,
    private loadService: LoaderService,
    private toastServer: ToastServer
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadService.show();
      this.questionService.getQuestionById(id).subscribe({
        next: (data) => {
          this.question = data;
          this.loadQuestionPhoto(this.question?.Id ?? '');
          this.loadUserPhoto(this.question?.User?.Id ?? '');

          this.question?.Answers?.forEach(answer => {
            this.loadAnswerUserPhoto(answer.User.Id);
          });

          this.loading = false;
          this.loadService.hide();
          this.toastServer.showToast('Question loaded successfully');
        },
        error: (err) => {
          console.error('Error fetching question:', err);
          this.loading = false;
          this.loadService.hide();
          this.toastServer.showToast('Failed to load question');
        }
      });
    }
  }

  selectFinalAnswer(answer: any) {
    if (!this.question || !this.isCurrentUserAuthor() || this.question.IsClosed) return;
  
    // Ako question i Answers nisu null
    this.question.Answers?.forEach(a => a.IsFinal = false);
  
    // Oznaci izabrani odgovor kao final
    answer.IsFinal = true;
  
    // Zatvori pitanje za dalje glasanje
    this.question.IsClosed = true;
  
    // Pozovi backend ako postoji endpoint
    if (this.question.Id && answer.Id) {
      this.questionService.markFinalAnswer(this.question.Id, answer.Id).subscribe({
        next: () => this.toastServer.showToast('Final answer selected successfully'),
        error: (err) => {
          console.error('Error marking final answer:', err);
          this.toastServer.showToast('Failed to select final answer');
        }
      });
    }
  }
  

  loadQuestionPhoto(id: string) {
    if (!id) {
      this.questionPhotoUrl = null;
      return;
    }
    this.questionService.getQuestionPhoto(id).subscribe({
      next: (blob) => {
        if (blob && blob.size > 0) {
          this.questionPhotoUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(blob));
        } else {
          this.questionPhotoUrl = null;
        }
      },
      error: () => {
        this.questionPhotoUrl = null;
      }
    });
  }

  loadUserPhoto(userId: string) {
    if (!userId) {
      this.userPhotoUrl = null;
      return;
    }
    this.questionService.getUserPhoto(userId).subscribe({
      next: (blob) => {
        if (blob && blob.size > 0) {
          this.userPhotoUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(blob));
        } else {
          this.userPhotoUrl = null;
        }
      },
      error: () => {
        this.userPhotoUrl = null;
      }
    });
  }
  
  loadAnswerUserPhoto(userId: string) {
    if (!userId) {
      this.answerUserPhotos[userId] = null;
      return;
    }
  
    this.questionService.getUserPhoto(userId).subscribe({
      next: (blob) => {
        if (blob && blob.size > 0) {
          this.answerUserPhotos[userId] = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(blob));
        } else {
          this.answerUserPhotos[userId] = null;
        }
      },
      error: () => {
        this.answerUserPhotos[userId] = null;
      }
    });
  }
  

  formatDate(date: string): string {
    return new Date(date).toLocaleString();
  }

  isCurrentUserAuthor(): boolean {
    if (!this.question) return false;
    const currentUserId = this.authService.getCurrentUserId();
    return this.question.UserId === currentUserId;
  }

  getTimeAgo(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const seconds = Math.floor((now.getTime() - date.getTime()) / 1000);

    let interval = Math.floor(seconds / 31536000);
    if (interval >= 1) {
      return interval === 1 ? "asked 1 year ago" : `${interval} years ago`;
    }

    interval = Math.floor(seconds / 2592000);
    if (interval >= 1) {
      return interval === 1 ? "asked 1 month ago" : `${interval} months ago`;
    }

    interval = Math.floor(seconds / 86400);
    if (interval >= 1) {
      if (interval === 1) return "asked yesterday";
      if (interval < 7) return `asked ${interval} days ago`;
      const weeks = Math.floor(interval / 7);
      return weeks === 1 ? "asked 1 week ago" : `${weeks} weeks ago`;
    }

    interval = Math.floor(seconds / 3600);
    if (interval >= 1) {
      return interval === 1 ? "asked 1 hour ago" : `${interval} hours ago`;
    }

    interval = Math.floor(seconds / 60);
    if (interval >= 1) {
      return interval === 1 ? "asked 1 minute ago" : `${interval} minutes ago`;
    }

    return "just now";
  }


  
  cancelEdit() {
    this.editMode = false;
    
    this.editedTitle = '';
    this.editedDescription = '';
    this.editedPhotoFile = null;
    this.editedPhotoUrl = this.originalPhotoUrl;
  }
  
  onPhotoSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.editedPhotoFile = input.files[0];
      this.editedPhotoUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(this.editedPhotoFile));
    }
  }

  removePhoto() {
    this.editedPhotoFile = null;
    this.editedPhotoUrl = null;
    this.questionPhotoUrl = null;
  }

  startEdit() {
    if (!this.question) return;
    this.editMode = true;
    this.editedTitle = this.question.Title;
    this.editedDescription = this.question.Description;

    this.editedPhotoFile = null;
    this.editedPhotoUrl = this.questionPhotoUrl;
    this.originalPhotoUrl = this.questionPhotoUrl;
  }

    titleError = false;
    descriptionError = false;
    photoError = false;
  
    saveEdit() {
      this.titleError = false;
      this.descriptionError = false;
      this.photoError = false;
    
      let valid = true;
      if (!this.editedTitle || this.editedTitle.trim() === '') {
        this.titleError = true;
        valid = false;
      }
      if (!this.editedDescription || this.editedDescription.trim() === '') {
        this.descriptionError = true;
        valid = false;
      }
      if (!this.editedPhotoUrl) {
        this.photoError = true;
        valid = false;
      }
    
      if (!valid) return;
    
      if (!this.question) return;
    
      this.loadService.show();
    
      this.questionService.updateQuestion(this.question.Id, this.editedTitle, this.editedDescription).subscribe({
        next: updatedQuestion => {
          this.question = updatedQuestion;
          this.editMode = false;
    
          if (this.editedPhotoFile) {
            this.questionService.uploadQuestionPhoto(this.question.Id, this.editedPhotoFile).subscribe({
              next: () => {
                this.loadQuestionPhoto(this.question?.Id ?? '');
                this.loadService.hide();
                this.toastServer.showToast('Question and photo updated successfully');
              },
              error: (err) => {
                this.loadService.hide();
                this.toastServer.showToast('Failed to upload photo');
                console.error('Failed to upload photo', err);
              }
            });
          } else if (this.editedPhotoUrl === null) {
            this.questionPhotoUrl = null;
            this.loadService.hide();
            this.toastServer.showToast('Question updated and photo removed successfully');
          } else {
            this.loadQuestionPhoto(this.question?.Id ?? '');
            this.loadService.hide();
            this.toastServer.showToast('Question updated successfully');
          }
        },
        error: err => {
          this.loadService.hide();
          this.toastServer.showToast('Failed to update question');
          console.error('Failed to update question', err);
        }
      });
    }
    
    deleteQuestion() {
      if (!this.question) return;
    
      if (!confirm('Are you sure you want to delete this question?')) return;
    
      this.loadService.show();
    
      this.questionService.deleteQuestion(this.question.Id).subscribe({
        next: () => {
          this.loadService.hide();
          this.toastServer.showToast('Question deleted successfully');
          window.location.href = '/questions';
        },
        error: err => {
          this.loadService.hide();
          this.toastServer.showToast('Failed to delete question');
          console.error('Failed to delete question', err);
        }
      });
    }
    

    isUpvoted(vote: number | null) {
      return vote === 1;
    }
    
    isDownvoted(vote: number | null) {
      return vote === -1;
    }
    vote(value: 1 | -1) {
      if (!this.question || !this.question.Id) return;
    
      const q = this.question;
      const originalVote = q.MyVote;
      const originalScore = q.VoteScore ?? 0;
    
      // Instant UI update
      if (originalVote === value) {
        q.MyVote = null;
        q.VoteScore = originalScore - value;
      } else {
        q.MyVote = value;
        q.VoteScore = originalScore - (originalVote ?? 0) + value;
      }
    
      // Mapiramo za backend
      const type: '+' | '-' = value === 1 ? '+' : '-';
    
      this.questionService.vote(q.Id, type).subscribe({
        next: () => this.toastServer.showToast('Vote submitted successfully'),
        error: (err) => {
          console.error('Failed to submit vote', err);
          // Rollback
          q.MyVote = originalVote;
          q.VoteScore = originalScore;
          this.toastServer.showToast('Failed to submit vote');
        }
      });
    }
    
    voteAnswer(answer: any, value: 1 | -1) {
      if (!this.question || !answer || !answer.Id) return;
    
      const originalVote = answer.MyVote as number | null;
      const originalScore = answer.VoteScore ?? 0;
    
      // Instant UI update
      if (originalVote === value) {
        answer.MyVote = null;
        answer.VoteScore = originalScore - value;
      } else {
        answer.MyVote = value;
        answer.VoteScore = originalScore - (originalVote ?? 0) + value;
      }
    
      const type: '+' | '-' = value === 1 ? '+' : '-';
    
      // Prosledjujemo i QuestionId i AnswerId
      this.questionService.voteAnswer(this.question.Id, answer.Id, type).subscribe({
        next: () => this.toastServer.showToast('Vote submitted successfully'),
        error: (err) => {
          console.error('Failed to submit answer vote', err);
          // Rollback
          answer.MyVote = originalVote;
          answer.VoteScore = originalScore;
          this.toastServer.showToast('Failed to submit vote');
        }
      });
    }
    

}

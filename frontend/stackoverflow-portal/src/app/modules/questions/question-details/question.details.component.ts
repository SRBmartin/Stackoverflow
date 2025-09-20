import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { QuestionResponseDto, QuestionDto } from '../../../core/dto/questions/question-response-dto';
import { AuthService } from '../../../core/auth/services/auth.service';
import { QuestionService } from '../../../core/services/question.service';
@Component({
  selector: 'app-question-details',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './question-details.component.html',
  styleUrls: ['./question-details.component.scss']
})
export class QuestionDetailsComponent implements OnInit {
  question: QuestionDto | null = null; // jedno pitanje
  loading = true;

  questionPhotoUrl: SafeUrl | null = null;
  userPhotoUrl: SafeUrl | null = null;

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private sanitizer: DomSanitizer, 
    private authService :AuthService, 
    private questionService : QuestionService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.questionService.getQuestionById(id).subscribe({
        next: (data) => {
          this.question = data;
          this.loadQuestionPhoto(this.question.Id);
          this.loadUserPhoto(this.question.User.Id);
          this.loading = false;
        },
        error: (err) => {
          console.error('Error fetching question:', err);
          this.loading = false;
        }
      });
    }
  }
  

  loadQuestionPhoto(id: string) {
    this.http.get(`/api/questions/${id}/photo`, { responseType: 'blob' }).subscribe({
      next: (blob) => {
        if (blob && blob.size > 0) {
          this.questionPhotoUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(blob));
        } else {
          this.questionPhotoUrl = null;  // nema slike
        }
      },
      error: () => {
        this.questionPhotoUrl = null;  // greška pri učitavanju - nema slike
      }
    });
  }
  

  loadUserPhoto(userId: string) {
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

  vote(value: 1 | -1) {
    if (!this.question) return;
  
    const currentVote = this.question.MyVote;
  
    if (currentVote === value) {
      // Kliknuto na isto dugme: poništi glas
      this.question.MyVote = null;
      this.question.VoteScore -= value;
    } else {
      // Ako je već glasano suprotno, ukloni stari glas
      if (currentVote) {
        this.question.VoteScore -= currentVote;
      }
      // Dodaj novi glas
      this.question.MyVote = value;
      this.question.VoteScore += value;
    }
  
    // Ovde možeš dodati poziv API-ja da sačuva glas na serveru
    // npr. this.questionService.vote(this.question.Id, value).subscribe();
  }
  
  

}

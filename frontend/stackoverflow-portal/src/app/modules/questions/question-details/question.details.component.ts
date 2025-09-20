import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-question-details',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './question-details.component.html',
  styleUrls: ['./question-details.component.scss']
})
export class QuestionDetailsComponent implements OnInit {
  question: any;
  loading = true;

  constructor(private route: ActivatedRoute, private http: HttpClient) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id'); // uzimamo :id iz rute
    if (id) {
      this.http.get(`/api/questions/${id}`).subscribe({
        next: (data) => {
          this.question = data;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error fetching question:', err);
          this.loading = false;
        }
      });
    }
  }
}

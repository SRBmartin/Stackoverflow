import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../env/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AnswerService {
  private readonly httpClient: HttpClient = inject(HttpClient);
  private readonly answersUrl = `${environment.apiUrl}/answers`;

  submitAnswer(questionId: string, text: string): Observable<any> {
    return this.httpClient.post(this.answersUrl, {
      QuestionId: questionId,
      Text: text
    });
  }

  markFinalAnswer(questionId: string, answerId: string): Observable<any> {
    return this.httpClient.post(`${this.answersUrl}/final`, {
      QuestionId: questionId,
      AnswerId: answerId
    });
  }
}

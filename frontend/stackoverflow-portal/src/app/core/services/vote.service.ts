import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../env/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class VoteService {
  private readonly httpClient: HttpClient = inject(HttpClient);
  private readonly votesUrl = `${environment.apiUrl}/votes`;

  voteQuestion(questionId: string, type: '+' | '-'): Observable<any> {
    return this.httpClient.post(this.votesUrl, {
      QuestionId: questionId,
      Type: type
    });
  }

  voteAnswer(questionId: string, answerId: string, type: '+' | '-'): Observable<any> {
    return this.httpClient.post(this.votesUrl, {
      QuestionId: questionId,
      AnswerId: answerId,
      Type: type
    });
  }
}

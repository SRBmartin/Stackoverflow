import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../env/environment';
import { Observable } from 'rxjs';
import { QuestionResponseDto, QuestionDto } from '../dto/questions/question-response-dto';
import { QuestionsSortBy } from '../../modules/questions/list-questions/const/questions-sort-by';
import { SortDirection } from '../../modules/questions/list-questions/const/sort-direction';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class QuestionService {
  private readonly httpClient: HttpClient = inject(HttpClient);
  private readonly questionsUrl = `${environment.apiUrl}/questions`;

  getQuestions(page: number, title: string = '', sortBy: QuestionsSortBy = QuestionsSortBy.Date, direction: SortDirection = SortDirection.Desc): Observable<QuestionResponseDto> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('title', title)
      .set('sortBy', sortBy.toString().toLowerCase())
      .set('direction', direction.toString().toLowerCase());

    return this.httpClient.get<QuestionResponseDto>(this.questionsUrl, { params });
  }

  getQuestionById(id: string): Observable<QuestionDto> {
    return this.httpClient.get<QuestionDto>(`${this.questionsUrl}/${id}`);
  }

  getQuestionPhoto(id: string): Observable<Blob> {
    return this.httpClient.get(`${environment.apiUrl}/questions/${id}/photo`, { responseType: 'blob' });
  }

  getUserPhoto(userId: string): Observable<Blob> {
    return this.httpClient.get(`${environment.apiUrl}/users/${userId}/photo`, { responseType: 'blob' });
  }

  updateQuestion(id: string, title: string, description: string): Observable<QuestionDto> {
    return this.httpClient.put<QuestionDto>(`${this.questionsUrl}/${id}`, { title, description });
  }
  
  deleteQuestion(id: string): Observable<void> {
    return this.httpClient.delete<void>(`${this.questionsUrl}/${id}`);
  }
  uploadQuestionPhoto(id: string, file: File): Observable<void> {
    const formData = new FormData();
    formData.append('file', file);
    return this.httpClient.post<void>(`${this.questionsUrl}/${id}/photo`, formData);
  }

  deleteQuestionPhoto(id: string): Observable<void> {
    return this.httpClient.delete<void>(`${this.questionsUrl}/${id}/photo`);
  } 

  markFinalAnswer(questionId: string, answerId: string) {
    return this.httpClient.post(
      `${environment.apiUrl}/answers/final`,
      {
        QuestionId: questionId,
        AnswerId: answerId
      }
    );
  }

  vote(questionId: string, type: '+' | '-') {
    return this.httpClient.post(`${environment.apiUrl}/votes`, {
      QuestionId: questionId,
      Type: type
    });
  }
  
  voteAnswer(questionId:string ,answerId: string, type: '+' | '-') {
    return this.httpClient.post(`${environment.apiUrl}/votes`, {
      QuestionId :questionId,
      AnswerId: answerId,
      Type: type
    });
  }

  submitAnswer(questionId: string, text: string) {
    return this.httpClient.post(`${environment.apiUrl}/answers`, {
      QuestionId: questionId,
      Text: text
    });
  }
  
}
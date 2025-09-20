import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../env/environment';
import { Observable } from 'rxjs';
import { QuestionResponseDto } from '../dto/questions/question-response-dto';
import { QuestionsSortBy } from '../../modules/questions/list-questions/const/questions-sort-by';
import { SortDirection } from '../../modules/questions/list-questions/const/sort-direction';

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
}
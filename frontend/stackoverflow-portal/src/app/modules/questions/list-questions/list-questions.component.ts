import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { QuestionResponseDto } from '../../../core/dto/questions/question-response-dto';
import { QuestionService } from '../../../core/services/question.service';
import { ToastServer } from '../../../common/ui/toast/toast.service';
import { LoaderService } from '../../../common/ui/loader/loader.service';
import { QuestionsSortBy } from './const/questions-sort-by';
import { SortDirection } from './const/sort-direction';
import { SearchService } from '../../../core/services/shared/search.service';
import { debounceTime, distinctUntilChanged, Subscription } from 'rxjs';

@Component({
  selector: 'app-list-questions',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule
  ],
  templateUrl: './list-questions.component.html',
  styleUrl: './list-questions.component.scss'
})
export class ListQuestionsComponent implements OnInit, OnDestroy {
  questions: QuestionResponseDto | null = null;
  currentPage: number = 1;
  searchTerm: string = '';
  sortBy: QuestionsSortBy = QuestionsSortBy.Date;
  direction: SortDirection = SortDirection.Desc;
  totalPages: number = 1;

  sortByOptions: string[] = Object.values(QuestionsSortBy);
  directionOptions: string[] = Object.values(SortDirection);

  private searchSubscription: Subscription | undefined;

  constructor(
    private readonly questionService: QuestionService,
    private readonly searchService: SearchService,
    private readonly toastService: ToastServer,
    private readonly loaderService: LoaderService
  ) {}

  ngOnInit(): void {
    this.loadQuestions();
    let firstLoad = true;
    this.searchSubscription = this.searchService.searchTerm$.pipe(
      debounceTime(1500),
      distinctUntilChanged()
    ).subscribe(term => {
      this.searchTerm = term.trim();
      this.currentPage = 1;

      if (firstLoad) {
        firstLoad = false;
        return;
      }

      if (this.searchTerm.length === 0 || this.searchTerm.length >= 3) {
        this.loadQuestions(); 
      }
    });
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }

  loadQuestions(): void {
    this.loaderService.show();
    this.questionService.getQuestions(this.currentPage, this.searchTerm, this.sortBy, this.direction).subscribe({
      next: (response) => {
        this.questions = response;
        this.totalPages = response.TotalPages;
        this.loaderService.hide();
      },
      error: (err) => {
        let errorMessage = err?.error?.message || 'Failed to load questions. Please try again.';
        this.toastService.showToast(errorMessage, 'error');
        this.loaderService.hide();
      }
    });
  }

  onPageChange(page: number | string): void {
    if (typeof page !== 'number' || page < 1 || page > this.totalPages) {
      return;
    }
    this.currentPage = page;
    this.loadQuestions();
  }

  onSortChange(): void {
    this.currentPage = 1; 
    this.loadQuestions();
  }

  getPageArray(): (number | string)[] {
    const pages: (number | string)[] = [];
    const maxVisiblePages = 5;

    if (this.totalPages <= maxVisiblePages) {
      for (let i = 1; i <= this.totalPages; i++) {
        pages.push(i);
      }
      return pages;
    }

    pages.push(1);

    let start = Math.max(2, this.currentPage - 1);
    let end = Math.min(this.totalPages - 1, this.currentPage + 1);

    if (this.currentPage > 3) {
      pages.push('...');
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    if (this.currentPage < this.totalPages - 2) {
      pages.push('...');
    }

    pages.push(this.totalPages);

    return pages;
  }

  getAnswersText(answerCount: number, isClosed: boolean): string {
    if (answerCount === undefined || answerCount === null) {
      return 'No answers yet';
    }
    let text = answerCount === 0 ? 'No answers yet' : `${answerCount} ${answerCount === 1 ? 'answer' : 'answers'}`;
    if (isClosed && answerCount > 0) {
      text = '✓ ' + text;
    }
    return text;
  }
}
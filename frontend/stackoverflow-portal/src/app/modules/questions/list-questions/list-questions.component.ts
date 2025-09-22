import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { QuestionResponseDto } from '../../../core/dto/questions/question-response-dto';
import { QuestionService } from '../../../core/services/question.service';
import { ToastServer } from '../../../common/ui/toast/toast.service';
import { LoaderService } from '../../../common/ui/loader/loader.service';
import { SearchInputComponent } from '../../../components/shared/ui/search-input/search-input.component';
import { QuestionsSortBy } from './const/questions-sort-by';
import { SortDirection } from './const/sort-direction';

@Component({
  selector: 'app-list-questions',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    SearchInputComponent
  ],
  templateUrl: './list-questions.component.html',
  styleUrl: './list-questions.component.scss'
})
export class ListQuestionsComponent implements OnInit {
  questions: QuestionResponseDto | null = null;
  currentPage: number = 1;
  searchTerm: string = '';
  sortBy: QuestionsSortBy = QuestionsSortBy.Date;
  direction: SortDirection = SortDirection.Desc;
  totalPages: number = 1;

  // Expose enum values as arrays
  sortByOptions: string[] = Object.values(QuestionsSortBy);
  directionOptions: string[] = Object.values(SortDirection);

  constructor(
    private readonly questionService: QuestionService,
    private readonly toastService: ToastServer,
    private readonly loaderService: LoaderService
  ) {}

  ngOnInit(): void {
    this.loadQuestions();
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
        console.error('Failed to load questions:', err);
        this.toastService.showToast('Failed to load questions. Please try again.', 'error');
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

  onSearchTermChanged(term: string): void {
    this.searchTerm = term;
    this.currentPage = 1; // Reset to first page on new search
    this.loadQuestions();
  }

  onSortChange(): void {
    this.currentPage = 1; // Reset to first page on sort change
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

  getAnswersText(answersLength: number): string {
    if (answersLength === 0) {
      return 'No answers yet';
    }
    return `${answersLength} ${answersLength === 1 ? 'answer' : 'answers'}`;
  }
}
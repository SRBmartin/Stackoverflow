import { AnswerDto } from "../answer-dto";
export interface QuestionResponseDto {
  Page: number;
  PageSize: number;
  TotalPages: number;
  TotalItems: number;
  Items: QuestionDto[];
}

export interface QuestionDto {
  Id: string;
  UserId: string;
  Title: string;
  Description: string;
  PhotoBlobName: string | null;
  PhotoContainer: string | null;
  CreationDate: string;
  IsClosed: boolean;
  IsDeleted: boolean;
  User: {
    Id: string;
    Name: string;
    Lastname: string;
    Email: string;
    PhotoBlobName: string | null;
    PhotoContainer: string | null;
  };
  Answers: AnswerDto[]; // Adjust type based on actual AnswerDto if available
  VoteScore: number;
  MyVote: number | null;
}
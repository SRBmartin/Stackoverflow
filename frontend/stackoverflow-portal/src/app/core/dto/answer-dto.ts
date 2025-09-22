export interface AnswerDto {
    Id: string;
    QuestionId: string;
    UserId: string;
    Text: string;
    CreationDate: string;
    IsFinal: boolean;
    IsDeleted: boolean;
    User: {
      Id: string;
      Name: string;
      Lastname: string;
      Email: string;
      PhotoBlobName: string | null;
      PhotoContainer: string | null;
    };
    UpVotes: number;
    DownVotes: number;
    VoteScore: number;
    MyVote: number | null;
  }
  
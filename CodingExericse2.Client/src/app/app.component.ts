import { Component, OnInit } from '@angular/core';
import { Comment } from './models/comment';
import { CommentsService } from './services/comments.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  static readonly BadRequestStatusCode = 400;
  static readonly CommentCharacterLimit = 140;

  commentList: Comment[] = [];
  username: string = "";
  comment: string = "";
  error: string = "";
  commentLength: number = AppComponent.CommentCharacterLimit;

  constructor(private commentsService: CommentsService) { }

  ngOnInit(): void {
    this.commentsService.getComments().subscribe(
      comments => {
        this.commentList.push(...comments);
        this.commentsService.startFeed();

        this.commentsService.commentFeed$.subscribe(comment => {
          this.commentList.unshift(comment);
        });
      }
    )
  }

  updateCommentLength(): void {
    this.commentLength = AppComponent.CommentCharacterLimit - this.comment.length;

    if(this.commentLength < 0) {
      this.commentLength = 0;
    }
  }

  postComment(): void {
    this.commentsService.postComment({ username: this.username, comment: this.comment }).subscribe(
      () => {
        this.username = "";
        this.comment = "";
        this.error = "";
        this.commentLength = AppComponent.CommentCharacterLimit;
      },
      response => {
        if(response.status == AppComponent.BadRequestStatusCode) {
          this.error = response.data.Message;
        }
      }
    );
  }
}

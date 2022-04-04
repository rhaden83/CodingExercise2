import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { AddCommentRequest } from '../models/add-comment-request';
import { Comment } from '../models/comment';

@Injectable({
  providedIn: 'root'
})
export class CommentsService {
  commentFeed$: Subject<Comment> = new Subject<Comment>();
  private hubConnection: HubConnection | undefined;

  constructor(private http: HttpClient) { }

  getComments(): Observable<Comment[]> {
    return this.http.get<Comment[]>("http://localhost:5000/api/comments");
  }

  startFeed(): void {
    this.hubConnection = new HubConnectionBuilder().withUrl('http://localhost:5000/api/comments/feed').build();

    this.hubConnection.start()
      .then(() => {
        console.log('Connection started.');

        this.hubConnection?.on("broadcast", data => {
          try {
            this.commentFeed$.next(Object.assign({} as Comment, JSON.parse(data)));
          } catch(error) {
            console.log('Error parsing comment JSON from feed: ' + error);
          }
        });
      })
      .catch(error => console.log('Error while starting connection: ' + error));
  }

  postComment(request: AddCommentRequest): Observable<any> {
    return this.http.post("http://localhost:5000/api/comments", request);
  }
}

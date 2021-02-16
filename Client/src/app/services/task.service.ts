import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Api } from '../models/api';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  constructor(private httpClient: HttpClient) { }

  getListApi(): Observable<Api[]>{
    return this.httpClient.get<Api[]>(`${environment.url}api/task/apies`);
  }

}

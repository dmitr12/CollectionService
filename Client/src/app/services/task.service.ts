import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Api } from '../models/api';
import { TaskModel } from '../models/taskModel';
import { TaskViewModel } from '../models/taskViewModel';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  constructor(private httpClient: HttpClient) { }

  getListApi(): Observable<Api[]>{
    return this.httpClient.get<Api[]>(`${environment.url}api/task/apies`);
  }

  getTasksByUserId(userId: number): Observable<TaskModel[]>{
    return this.httpClient.get<TaskModel[]>(`${environment.url}api/task/GetTasksByUserId/${userId}`);
  }

  deleteTask(idTask: number){
    return this.httpClient.delete(`${environment.url}api/task/DeleteTask/${idTask}`);
  }

  addTask(taskModel: TaskViewModel){
    return this.httpClient.post(`${environment.url}api/task/addtask`, taskModel);
  }
}

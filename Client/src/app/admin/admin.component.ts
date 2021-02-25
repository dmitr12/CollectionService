import {Component, OnInit} from '@angular/core';
import {UserTasksInfo} from '../models/userTasksInfo';
import {TaskService} from '../services/task.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {

  statistics: any;

  constructor(private taskService: TaskService) {
  }

  ngOnInit(): void {
    this.reload();
  }

  reload() {
    this.taskService.getStatistics().subscribe((res: UserTasksInfo[]) => {
      this.statistics = res;
    }, error => {
      alert('Статусный код ' + error.status);
    });
  }
}

import {Component, OnInit, ViewChild} from '@angular/core';
import {MatDialog, MatDialogConfig} from '@angular/material/dialog';
import {UserAuthentication} from 'src/app/models/userAuthentication';
import {AuthenticationService} from 'src/app/services/authentication.service';
import {TaskService} from 'src/app/services/task.service';
import {AddTaskComponent} from '../add-task/add-task.component';
import {DeleteTaskComponent} from '../delete-task/delete-task.component';
import {UpdateTaskComponent} from '../update-task/update-task.component';
import {CronGenComponent, CronOptions} from 'ngx-cron-editor';
import {FormControl} from "@angular/forms";

@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent implements OnInit {

  dialogSource: any;

  constructor(private dialog: MatDialog, private taskService: TaskService,
              private authenticationService: AuthenticationService) {
  }

  tasks: any;

  add() {
    const dialogConfig = new MatDialogConfig();
    this.dialogSource = this.dialog.open(AddTaskComponent, dialogConfig);
    this.dialogSource.afterClosed().subscribe((result: boolean) => {
      if (result != false) {
        this.reload();
      }
    });
  }

  editTask(idTask: number) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = idTask;
    this.dialogSource = this.dialog.open(UpdateTaskComponent, dialogConfig);
    this.dialogSource.afterClosed().subscribe((result: boolean) => {
      if (result != false) {
        this.reload();
      }
    });
  }

  ngOnInit(): void {
    this.reload();
  }

  reload() {
    this.taskService.getTasksByUserId(this.authenticationService.getCurrentUserId()).subscribe(res => {
      this.tasks = res;
    }, error => {
      alert('Статусный код ' + error.status);
    });
  }

  deleteTask(idTask: number) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = idTask;
    this.dialogSource = this.dialog.open(DeleteTaskComponent, dialogConfig);
    this.dialogSource.afterClosed().subscribe((result: boolean) => {
      if (result != false) {
        this.reload();
      }
    });
  }
}

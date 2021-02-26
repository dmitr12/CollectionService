import {Component, Inject, OnInit} from '@angular/core';
import {MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import {TaskService} from 'src/app/services/task.service';

@Component({
  selector: 'app-delete-task',
  templateUrl: './delete-task.component.html',
  styleUrls: ['./delete-task.component.css']
})
export class DeleteTaskComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              private dialogSource: MatDialogRef<DeleteTaskComponent>, private taskService: TaskService) {
  }

  ngOnInit(): void {
  }

  delete() {
    this.taskService.deleteTask(this.data).subscribe(res => {
      this.dialogSource.close();
    }, error => {
      alert('Статусный код ' + error.status);
    });
  }

}

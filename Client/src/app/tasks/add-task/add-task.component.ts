import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Api } from 'src/app/models/api';
import { TaskService } from 'src/app/services/task.service';

@Component({
  selector: 'app-add-task',
  templateUrl: './add-task.component.html',
  styleUrls: ['./add-task.component.css']
})
export class AddTaskComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
  private dialogSource: MatDialogRef<AddTaskComponent>, private taskService: TaskService) { }

  formAdd = new FormGroup({
    name: new FormControl(null, [Validators.required]),
    description: new FormControl(null, [Validators.required]),
    periodicityMin: new FormControl(null, [Validators.required, Validators.min(5)]),
    apiId: new FormControl(null, [Validators.required]),
    filterText: new FormControl(null, [Validators.required])
  })

  apiList: any;
  selectedApi = '';

  ngOnInit(): void {
    this.taskService.getListApi().subscribe((result: Api[])=>{
      this.apiList=result;
    }, error=>{
      alert(error.message);
    });
  }

  add(){

  }
}

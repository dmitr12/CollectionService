import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Api } from 'src/app/models/api';
import { TaskViewModel } from 'src/app/models/taskViewModel';
import { TaskService } from 'src/app/services/task.service';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-add-task',
  templateUrl: './add-task.component.html',
  styleUrls: ['./add-task.component.css']
})
export class AddTaskComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
  private dialogSource: MatDialogRef<AddTaskComponent>, private taskService: TaskService,
  public datepipe: DatePipe) { }

  formAdd = new FormGroup({
    name: new FormControl(null, [Validators.required]),
    description: new FormControl(null, [Validators.required]),
    periodicityMin: new FormControl(null, [Validators.required, Validators.min(2)]),
    apiId: new FormControl(null, [Validators.required]),
    filterText: new FormControl(null, [Validators.required]),
    startDate: new FormControl(this.datepipe.transform(new Date, 'yyyy-MM-dd'), [Validators.required]),
    startTime: new FormControl(null, [Validators.required])
  })

  apiList: any;
  selectedApi = '';
  date=new Date();


  taaaa: any;


  ngOnInit(): void {
    this.taskService.getListApi().subscribe((result: Api[])=>{
      this.apiList=result;
    }, error=>{
      alert(error.message);
    });
  }

  add(){
    this.taskService.addTask(new TaskViewModel(this.formAdd.value.name, this.formAdd.value.description,
      `${this.formAdd.value.startDate} ${this.formAdd.value.startTime}`, this.formAdd.value.periodicityMin,
      this.formAdd.value.filterText, this.formAdd.value.apiId)).subscribe((res:any)=>{
        if(!res['msg']){
          this.dialogSource.close();
        }else{
          alert(res['msg'])
        }
      }, error =>{
        alert("Статусный код "+error.status);
      })
  }
}

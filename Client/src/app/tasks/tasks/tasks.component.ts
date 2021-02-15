import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { UserAuthentication } from 'src/app/models/userAuthentication';
import { AddTaskComponent } from '../add-task/add-task.component';

@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent implements OnInit {

  dialogSource: any;

  constructor(private dialog: MatDialog) { }

  tasks=[{
    "name":"name1",
    "description": "desc1",
    "last": "last"
  },{
    "name":"name1",
    "description": "desc1",
    "last": "last"
  }]

  testOpenDialog(){
    const dialogConfig=new MatDialogConfig();
    dialogConfig.width = "40%";
    dialogConfig.data=new UserAuthentication('assssda','asd');
    this.dialogSource = this.dialog.open(AddTaskComponent, dialogConfig);
    this.dialogSource.afterClosed().subscribe((result: boolean)=>{
      if(result){
        alert('hello')
      }   
    })
  }

  ngOnInit(): void {
  }

}

import {DatePipe} from '@angular/common';
import {Component, Inject, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {ErrorStateMatcher} from '@angular/material/core';
import {MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import {Api} from 'src/app/models/api';
import {TaskModel} from 'src/app/models/taskModel';
import {TaskViewModel} from 'src/app/models/taskViewModel';
import {TaskService} from 'src/app/services/task.service';
import {AddTaskComponent} from '../add-task/add-task.component';
import {CronOptions} from 'ngx-cron-editor';

@Component({
  selector: 'app-update-task',
  templateUrl: './update-task.component.html',
  styleUrls: ['./update-task.component.css']
})
export class UpdateTaskComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              private dialogSource: MatDialogRef<UpdateTaskComponent>, private taskService: TaskService,
              public datepipe: DatePipe) {
  }

  formUpd = new FormGroup({
    name: new FormControl(null, [Validators.required]),
    description: new FormControl(null, [Validators.required]),
    periodicityMin: new FormControl(null, [Validators.required]),
    apiId: new FormControl(null, [Validators.required]),
    filterText: new FormControl(null, [Validators.required]),
    startDate: new FormControl(null, [Validators.required]),
    startTime: new FormControl(null, [Validators.required])
  });
  apiList: any;
  selectedApi = '';
  date = new Date();
  selectedTask: any;

  public cronOptions: CronOptions = {
    formInputClass: 'form-control cron-editor-input',
    formSelectClass: 'form-control cron-editor-select',
    formRadioClass: 'cron-editor-radio',
    formCheckboxClass: 'cron-editor-checkbox',
    defaultTime: '',
    hideMinutesTab: false,
    hideHourlyTab: false,
    hideDailyTab: false,
    hideWeeklyTab: false,
    hideMonthlyTab: false,
    hideYearlyTab: true,
    hideAdvancedTab: true,
    hideSpecificWeekDayTab: false,
    hideSpecificMonthWeekTab: false,
    use24HourTime: true,
    hideSeconds: false,
    cronFlavor: 'quartz'
  };

  formStr = '* * * * * *';

  ngOnInit(): void {

    this.taskService.getListApi().subscribe((result: Api[]) => {
      this.apiList = result;
    }, error => {
      alert(error.message);
    });

    this.taskService.getTaskById(this.data).subscribe(res => {
      this.selectedTask = res;
      this.formUpd.controls.name.setValue(res.name);
      this.formUpd.controls.description.setValue(res.description);
      this.formUpd.controls.periodicityMin.setValue(res.periodicity);
      this.formUpd.controls.apiId.setValue(res.apiId);
      this.formUpd.controls.filterText.setValue(res.filterText);
      this.formUpd.controls.startDate.setValue(res.startTask.split(' ')[0]);
    }, error => {
      alert('Статусный код ' + error.status);
    });

    this.formUpd.controls.apiId.valueChanges.subscribe(() => {
      this.formUpd.controls.filterText.setValue('');
    });
  }

  update() {
    const model = new TaskViewModel(this.formUpd.value.name, this.formUpd.value.description,
      `${this.formUpd.value.startDate} ${this.formUpd.value.startTime}`, this.formUpd.value.periodicityMin,
      this.formUpd.value.filterText, this.formUpd.value.apiId);
    model.taskId = this.data;
    this.taskService.updateTask(model).subscribe((res: any) => {
        this.dialogSource.close();
    }, error => {
      if (error.status === 400) {
        alert('Отправлены неверные данные');
      }
      if (error.status === 500) {
        alert('Возникла ошибка на сервере');
      }
    });
  }
}

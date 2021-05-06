import { Component, HostListener, OnInit } from '@angular/core';

@Component({
  selector: 'app-back-top',
  templateUrl: './back-top.component.html',
  styleUrls: ['./back-top.component.css']
})
export class BackTopComponent {

  isShow: boolean;
  topPosToStartShowing = 50;

  @HostListener('window:scroll')
  checkScroll() {
    const scrollPosition = window.pageYOffset ||
    document.documentElement.scrollTop ||
    document.body.scrollTop || 0;

    //console.log('[scroll]', scrollPosition);

    if (scrollPosition >= this.topPosToStartShowing) {
      this.isShow = true;
    } else {
      this.isShow = false;
    }
  }

  // TODO: Cross browsing
  gotoTop() {
    window.scroll({
      top: 0,
      left: 0,
      behavior: 'smooth'
    });
  }

}

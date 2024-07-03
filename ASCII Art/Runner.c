#pragma comment(lib, "WINMM.LIB")
#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <mmsystem.h>
#include <string.h>
#include <time.h>
#define N 1000

float fps, fl[4];
char st[N], fw[5], fh[5], cfg[30], p[50];
char *retptr, *ptr;
int w, h, a, s, is = 0, caf = 33;
void recursur()
{
    HANDLE hout;
    COORD coord;
    coord.X = 0;
    coord.Y = 0;
    hout = GetStdHandle(STD_OUTPUT_HANDLE);
    SetConsoleCursorPosition(hout, coord);
}
void wrtPage(FILE *fp)
{
    for (int i = 0; i < h; i++)
    {
        fgets(st, N, fp);
        printf("%s", st);
    }
}
/*
    fl[0]  :  Frame count
    fl[1]  :  Width
    fl[2]  :  Height
    fl[3]  :  Fps
*/
int main()
{
    clock_t stime = 0, ftime = 0;
    FILE *fp = fopen(".cache\\1.dat", "r");
    fgets(cfg, 30, fp);
    ptr = cfg;
    while ((retptr = strtok(ptr, ",")) != NULL)
    {
        fl[is] = atof(retptr);
        ptr = NULL;
        is++;
    }
    a = fl[0];
    w = (int)fl[1];
    h = (int)fl[2];
    fps = fl[3];
    s = 0;
    itoa(w, fw, 10);
    itoa(h + 1, fh, 10);
    strcpy(p, "mode con cols=");
    strcat(p, fw);
    strcat(p, " lines=");
    strcat(p, fh);
    system(p);
    system("cls");
    mciSendString("open .cache\\2.mp3 alias bkmusic", NULL, 0, NULL);
    mciSendString("play bkmusic", NULL, 0, NULL);
    Sleep(1350);
    stime = clock();
    while (fl[0])
    {
        if (s % (int)fps == 0 && fl[0] != a)
        {
            caf += 1000 - (int)(caf * fps);
            s++;
        }
        else
            caf = (int)(1000.0 / fps);
        if (s % 10 == 0)
            caf -= (int)(10 * fps * caf) % 10;
        if (s % 100 == 0)
            caf -= (int)(100 * fps * caf) % 10;
        ftime = clock();
        if ((ftime - stime) >= caf)
        {
            wrtPage(fp);
            stime += caf;
            recursur();
            fl[0]--;
        }
    }
    return 0;
}

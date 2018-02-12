#include <unistd.h>
#include <stdlib.h>
#include <stdio.h>
#include <errno.h>
#include <sys/wait.h>
#include <fcntl.h>
#include <cstring>

const int  __COULD_NOT_CREATE_CHILD_PROCESS = 42;
const int  __MAX_ARGUMENTS = 24;
const int  __PIPE_ERROR = 12;

void _1();
void _5(char *function, char *arguments[]);
void _7();
void _9();
void _10();
void _11();
void _12();
void _14();
void _15();
void _17();
void _18(char *arguments[]);
void _19();
void _23();
void _pipe1();
void _pipe2();
void _pipe3(int, char*[]);
void _pipe4(int, char*[]);
void _files1(int, char*[]);

int main(int argc, char *argv[]) {

 //   if(freopen("exercises.txt", "w", stdout)) {
 //       _1();
 //       _5(argv[1], argv);
 //       _7();
 //       _9();
 //		  _10();
 //		  _11();
 //       _12();
 //		  _14();
 //		  _15();
 //       _17();
 //		  _18(argv);
 //       _19();
 //       _23();
 //       _pipe1();
 //       _pipe2();
 //       _pipe3(argc, argv);
 //       _pipe4(argc, argv);
        _files1(argc, argv);
 //   }
    return 0;
}

void _1() {
    int pid = fork();

    if(pid < 0) {
        printf("Failed to create child process");
        exit(__COULD_NOT_CREATE_CHILD_PROCESS);
    }
    else {
        if(pid != 0) {
            printf("We are in the child process");
            int error = execlp("myFile", "amazingArgument", (char *)NULL);
            printf("Execlp failed with error: %d. Lets call it properly.", error);
            execlp("date", "date", (char *)NULL);
        }
        else {
            printf("We are in the parent process, lets wait for the children.");

            int status;
            waitpid(pid, &status, 0);
            printf("\n%d", status);

            exit(0);
        }
    }
}
void _5(char *function, char *arguments[]) {
    execvp(function, &arguments[1]);
}
void _7() {
    int pid = fork();
    if (pid==0){
        sleep(5);
        printf("Child: PID is %d\n",getpid());
        printf("Child: Ppid is %d\n",getppid());
    }
    else
    {
        printf("Parent: PID is %d\n",getpid());
        printf("Parent: Ppid is %d\n",getppid());
        wait(0);
    }

    printf("Obshti deistviq\n");
}
void _9() {
    int i = 0;
    int pid = fork();
    if(pid == 0) {
        i++;
        printf("Child i=%d\n", i);
    }
    else {
        i+= 10;
        printf("Parent i=%d\n", i);
    }
    i+=2;
    printf("Common i=%d\n", i);
}
void _10() {
    int i = 0;
    int pid = fork();
    if(pid == 0) {
        i++;
        printf("Child i=%d\n", i);
		exit(0);
    }
    else {
        i+= 10;
        printf("Parent i=%d\n", i);
    }
    i+=2;
    printf("Common i=%d\n", i);
}
void _11() {
    int i = 0;
    int pid = fork();
    if(pid == 0) {
        i++;
        printf("Child i=%d\n", i);
		exit(0);
    }
    else {
        i+= 10;
        printf("Parent i=%d\n", i);
		exit(0);
    }
    i+=2;
    printf("Common i=%d\n", i);
}
void _12() {

    FILE* file = fopen("task-12.txt", "w");

    int pid = fork();
    if(pid == 0) {
        fprintf(file, "Printing from child.\n");
    }
    else {
        fprintf(file, "Printing from parent.\n");
		wait(0);
    }

    fprintf(file, "Printing from common.\n");
}
void _14() {

    FILE* file = fopen("task-14.txt", "w");

    int pid = fork();
    if(pid == 0) {
		execlp("date","date",(char *)NULL);
    }
    else {
		wait(0);
        fprintf(file, "Printing from parent.\n");
    }

    fprintf(file, "Printing from common.\n");
}
void _15() {

    FILE* file = fopen("task-15.txt", "w");

    int pid = fork();
    if(pid == 0) {
		execlp("myCOmmand","amazingInDeed",(char *)NULL);
    }
    else {
		wait(0);
        fprintf(file, "Printing from parent.\n");
    }

    fprintf(file, "Printing from common.\n");
}
void _17() {
    close(1);
    creat("task-17.txt", 0777);
    write(1, "Printing from common.\n", 22);
}
void _18(char* arguments[]) {
    char buff[] = "Printing from common.\n";
    close(1);
    creat(arguments[1], 0777);
    write(1, buff, sizeof(buff)/ sizeof(char));
}
void _19() {
    int fd;
    char buff[] = "Amazing writing.\n";

    if((fd = open("task-19.txt", O_WRONLY|O_CREAT)) != -1) {
        close(1);
        dup(fd);
        write(fd, buff, sizeof(buff)/ sizeof(char));
    }
    else {
        printf("Something is wrong.");
    }
}
void _23() {
    int fd;

    if((fd = open("task-23.txt", O_RDWR| O_CREAT)) != -1) {
        for (int i = 97; i < 123 ; ++i) {
            write(fd, "%s", (char)i);
        }
    }

    //lseek(fd, 15, SEEK_SET);

    char buff[5];
    read(fd, buff, 5);
    printf("%s", buff);
}

// const int __PIPE_ERROR = 45;
const int __WRITE_ERROR = 44;
const int __READ_ERROR = 43;
const int __EXEC_ERROR = 46;
const int __WRONG_NUMBER_ARGUMENTS = 47;
const int __OPEN_ERROR = 48;

const int READ = 0;
const int WRITE = 1;
const int MAX = 1024;

void _pipe1() {
    int fd[2], n;
    char buff[MAX];

    if(pipe(fd) < 0)
        exit(__PIPE_ERROR);
    printf("Hello, from pipe: write: %d and read: %d\n", fd[WRITE], fd[READ]);

    if(write(fd[WRITE], "Hello World\n", 12) != 12) {
        printf("Explanation: %i", errno);
        exit(__WRITE_ERROR);
    }

    if((n = read(fd[READ], buff, MAX)) <= 0) {
        printf("Explanation: %i", errno);
        exit(__READ_ERROR);
    }

    write(1, buff, n);
    exit(0);
}
void _pipe2() {
    int pd[2], pid, status;

    if((pid = fork()) < 0)
        exit(__COULD_NOT_CREATE_CHILD_PROCESS);

    if(pid == 0) {
        if(pipe(pd) < 0)
            exit(__PIPE_ERROR);
        if((pid = fork()) < 0)
            exit(__COULD_NOT_CREATE_CHILD_PROCESS);
        else if(pid == 0) {
            dup2(pd[WRITE], 1);
            close(pd[READ]);
            close(pd[WRITE]);
            execlp("ls", "ls", (char *)NULL);
            exit(__EXEC_ERROR);
        } else {
            dup2(pd[READ], 0);
            close(pd[READ]);
            close(pd[WRITE]);
            execlp("wc", "wc", "-l", (char *)NULL);
            exit(__EXEC_ERROR);
        }
    }
    wait(&status);
    printf("Status after ending pipe: %d\n", status);
    exit(0);
}
void _pipe3(int argc, char* arguments[]) {

    if(argc < 3) {
        printf("Wrong usage. Example: ./a.out ls wc");
        exit(__WRONG_NUMBER_ARGUMENTS);
    }

    int pid, pd[2], status;

    if((pid = fork()) < 0)
        exit(__COULD_NOT_CREATE_CHILD_PROCESS);

    if(pid == 0) {
        if(pipe(pd) < 0)
            exit(__PIPE_ERROR);

        if((pid = fork()) < 0)
            exit(__COULD_NOT_CREATE_CHILD_PROCESS);

        else if(pid == 0) {
            dup2(pd[WRITE], WRITE);
            close(pd[WRITE]);
            close(pd[READ]);
            execlp(arguments[1], arguments[1], (char *)NULL);
            exit(__EXEC_ERROR);
        } else {
            dup2(pd[READ], READ);
            close(pd[READ]);
            close(pd[WRITE]);
            execlp(arguments[2], arguments[2], (char *)NULL);
            exit(__EXEC_ERROR);
        }
    }

    wait(&status);
    printf("Finished with status: %d\n", status);
    exit(0);
}
void _pipe4(int argc, char* arguments[]) {
    if(argc < 2)
        exit(__WRONG_NUMBER_ARGUMENTS);

    int pid, pd[2], status;

    if((pid = fork()) < 0) {
        if(pid == 0) {
            if(pipe(pd) < 0)
                exit(__PIPE_ERROR);

            if((pid = fork()) < 0)
                exit(__COULD_NOT_CREATE_CHILD_PROCESS);

            else if(pid == 0) {
                char buff[10];
                scanf("%s", buff);
                dup2(pd[READ], READ);
                close(pd[READ]);
                close(pd[WRITE]);
                execlp(buff, buff, (char *)NULL);
                exit(__EXEC_ERROR);
            } else {
                dup2(pd[WRITE], WRITE);
                close(pd[WRITE]);
                close(pd[READ]);
                execlp(arguments[1], arguments[1], arguments[2], (char *)NULL);
                exit(__EXEC_ERROR);
            }
        }

        wait(&status);
        printf("Pipe ended with status: %d\n", status);
        exit(0);
    }
}
void _pipe5() {
    int pid, pd[2], status;

    if((pid = fork()) < 0)
        exit(__COULD_NOT_CREATE_CHILD_PROCESS);
    if(pid == 0) {
        if(pipe(pd) < 0)
            exit(__PIPE_ERROR);

        if((pid = fork()) < 0)
            exit(__COULD_NOT_CREATE_CHILD_PROCESS);
        if(pid == 0) {
            if((pid = fork()) < 0)
                exit(__COULD_NOT_CREATE_CHILD_PROCESS);

            if(pid == 0) {
                dup2(pd[READ], READ);
                close(pd[READ]);
                close(pd[WRITE]);
                execl("who", "who", (char *)NULL);
                exit(__EXEC_ERROR);
            }

            dup2(pd[WRITE], WRITE);
            dup2(pd[READ], READ);
            close(pd[READ]);
            close(pd[WRITE]);
            execl("wc", "wc", (char *)NULL);
            exit(__EXEC_ERROR);
        }

        dup2(pd[WRITE], WRITE);
        close(pd[WRITE]);
        close(pd[READ]);
        execl("wc", "wc", "c", (char *)NULL);
        exit(__EXEC_ERROR);
    }

    wait(&status);
    printf("Finished with status: %d\n", status);
    exit(0);
}

void _files1(int argc, char* argv[]) {

    if(argc == 1) {
        char buf[3];
        int count = 0, lines = 0;
        while(read(0, buf, 1) != 0) {
            count++;
            if(buf[0] == '\n')
                lines++;
        }

        printf("Count:%d\n", count);
        printf("Lines:%d\n", lines);
    }
    else {
        int fd, bytes = 0;
        char buf[3];

        for (int arg = 1; arg < argc; arg++) {
            if ((fd = open(argv[arg], O_RDONLY) < 0)) {
                exit(__OPEN_ERROR);
            }
            while (read(fd, buf, 1) > 0) {
                printf("%s", buf);
                bytes++;
            }

            printf("%d %s", bytes, argv[arg]);
            bytes = 0;
            close(fd);
        }
    }

    exit(0);
}

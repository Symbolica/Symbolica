#include <string.h>

int main()
{
    char buff[10] = {0};
    strcpy(buff, "This string will overflow the buffer.");

    return 0;
}

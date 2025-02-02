using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    /*Delegate 대리자*/
    /*함수 자체를 인자로 넘겨주고, 넘김 받은 함수에서 넘겨준 함수를 Call Back*/
    /*delegate는 체이닝할 수 있다 > 한번의 실행으로 여러개의 함수 호출*/

    class Delegate_ST
    {
        /*  실생활로 빗대어 예시
         *  
            업체 사장님과 통화
            > 비서님께 먼저 통화 연결
            > 나의 연락처, 용건 알려주고, 연락 달라고
        */
        /*UI에서 많이 사용*/

        //아래 ButtonPressed() 메소드(함수)처럼 만들기 힘든 경우가 많다
        /*
         설계 부분에서의 이유 > UI 코드와 게임 로직 코드는 분리하는 게 좋다
         현실적인 부분에서의 이유 > 함수 내부 수정하지 못하는 경우(ex.유니티에서 제공하는 ButtonPressed)
        */
        static void ButtonPressed()
        {
            // PlayerAttack();
        }


        /*
         * 1. 매개변수 느낌으로 들어갈 함수를 만들고(delegate)
         * 2. Main에 호출될 함수를 만들고
         * 3. 매게변수로서 이용될 함수를 만들고*/

        //실제 사용 예시
        delegate int OnClicked();   //함수가 아니고 "형식"이다
                                    /*delegate를 딱보면
                                    "오 delegate ""형식""이네, 함수 자체를 넘겨주는 친구지"
                                    반환은 int , 입력 : void(매게변수 없음)
                                    delegate 형식 이름은 OnClicked네*/

        static void ZZin_BtnPressed(OnClicked clicked)
        {
            clicked();
        }

        static int zzin_BtnWork()
        {
            Console.WriteLine("testte");
            return 0;
        }

        static int hoho()
        {
            Console.WriteLine("hoho");
            return 0;
        }


        static void Main(string[] args)
        {
            



            //구동 되는 방식(자세하게)
            OnClicked test = new OnClicked(zzin_BtnWork); //zzin_Btn메소드(함수)를 지닌 OnClicked를 객체화하고
            test();

            test += hoho;
            ZZin_BtnPressed(zzin_BtnWork);

            //ZZin_BtnPressed(test);

        }
    }
}

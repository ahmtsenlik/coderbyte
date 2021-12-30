using System;

namespace calculator
{
    class Program
    {
        static void Main(string[] args)
        {          
        /*
        1+1+1+2
        7-2+(5-2)
        (4/2)(3-1)
        6-3/1
        45-10*2-1
        100*2
        812/2*(5-3)
        7-4-1+8(3)/2
        (5-2*0-9*0)*0
        8-7*(12+100/2)*9-2
        */
            string str="2*7/(2+5)/(1)";
            string firstString,lastString;
            int result=0;
            char chr;
            int firstBracket=0;
    
            //bu döngü parantezleri bulmak için karakterleri geziyor.
            for(int i=0;i<str.Length;i++)   
            {   
                //karakter seçildi
                chr=Convert.ToChar(str.Substring(i,1));

                //parantez tespit ediliyor
                if(chr=='(') 
                {
                   firstBracket =i;              
                }
                //en içteki parantezi bulmuş olduk
                else if(chr==')') 
                {
                    //tespit edilen kısım kesiliyor parca değişkenine atanıyor
                    string parca=str.Substring(firstBracket+1,i-firstBracket-1);

                     //islem metoduna parca gönderiliyor 
                    result=processPriority(parca);
                    firstString=str.Substring(0,firstBracket);
                    if(firstString!="")
                    {
                        //burada eğer 6+5(15-1) şeklinde bir ifade gelirse 6+5 ile (15-1) arasına çarpma sembolü ekler
                        if(Char.IsDigit(Convert.ToChar(firstString.Substring(firstString.Length-1,1))))
                        {
                            firstString=firstString+"*";
                        }
                    }
                    lastString=str.Substring(i+1,str.Length-1-i);
                    if(lastString!="")
                    {
                        //burada eğer (15-5)4+1 şeklinde bir ifade gelirse (15-5) ile 4+1 arasına çarpma sembolü ekler veya (15-3)(4+1) bu
                        if(lastString.Substring(0,1)=="("|| Char.IsDigit(Convert.ToChar(lastString.Substring(0,1))))
                        {
                            lastString="*"+lastString;
                        }
                    }
                    
                     //dönen sonuc parantezleri atılıp konumuna yerleştirilmiş oluyor.
                    str=firstString+result+lastString;

                    //başka parantez varsa onu işleme almak için döngüyü baştan başlatıyor
                    i=0;                 
                }         
            }
            // processPriority metoduna parantezlerden arıldırılmış işlem gönderiliyor.
            result= Convert.ToInt32(processPriority(str));
            Console.WriteLine(result);
            
        }
        //iki parantez arasındaki işlem yapılıyor

        //örnek buraya gelen string ifade= "20-3*15-1"
        public static int processPriority(string str)
        {
            int result=0;
            char chr;
            int previousNumber,nextNumber;
            string firstString,lastString;
            int searchIndex=0;

            if(str.Length==1)
            {   
                result=Convert.ToInt32(str);
                return result;
            }
            //öncelikli işlemlerimiz çarpma ve bölme olduğu için burada bu işlem gerçekleşiyor
            for(int i=0;i<str.Length;i++) 
            {
                //sırayla her karakteri geziyoruz
                chr=Convert.ToChar(str.Substring(i,1));

                //bir işlem sembolüne rastladık
                if(chr=='/'||chr=='*')
                 {
                     //findBeforeNumber metoduna işlem sembolünden önceki parçayı gönderiyoruz yani "20-3" dönen sonuc 3 olacak 
                    previousNumber=findBeforeNumber(i,str.Substring(0,i));

                    //findAfterNumber metoduna işlem sembolünden sonraki parçayı gönderiyoruz yani "15-1" dönen sonuc 15 olacak
                    nextNumber=findAfterNumber(str.Substring(i+1,str.Length-i-1));

                    //işlemimiz bölme ise burası çalışacak
                    if(chr=='/') 
                    {                
                        result= previousNumber/nextNumber;
                        string strIslem=previousNumber.ToString()+"/"+nextNumber.ToString();
                        searchIndex=str.IndexOf(previousNumber+"/"+nextNumber,0);
                        firstString=str.Substring(0,searchIndex);
                        lastString=str.Substring(strIslem.Length+firstString.Length,str.Length-(firstString.Length+strIslem.Length));
                        str=firstString+result+lastString;                                 
                        i=0;
                    }

                    //işlemimiz çarpma ise burası çalışacak örneğimizde işlemimiz çarpma 
                    else if(chr=='*') 
                    {
                        //burada previousNumber değilkenine 3 gelmişti nextNumber değişkenine 15 result değişkeninde sonucumuz 45 olacak
                        result= previousNumber*nextNumber; 

                        //burada işlemi string olarak görmek istiyorum çünkü uzunluğunu kullanmam gerek strIslem="3*15"
                        string strIslem=previousNumber.ToString()+"*"+nextNumber.ToString(); 

                        //bu işlemin str içindeki konumunu buluyorum yani 20-3*15-1 içindeki konumu
                        searchIndex=str.IndexOf(previousNumber+"*"+nextNumber,0);

                        // burada işlemden önceki parçayı kesiyorum yani 20- ifadesi 
                        firstString=str.Substring(0,searchIndex);

                        //burada işlemden sonraki parçayı kesiyorum -1 ifadesi
                        lastString=str.Substring(strIslem.Length+firstString.Length,str.Length-(firstString.Length+strIslem.Length));

                        //burada parçaları birleştirerek yeni string elde ediyorum str=20-45-1
                        str=firstString+result+lastString; 

                        //başka bir çarpma veya bölme işlemi için döngüyü tekrar başlatıyorum
                        i=0;                
                    }
                }

            }

            //burada işlemin soldan sağa doğru toplama çıkarma işlemini yaptığı bölüm yukarıdakiyle aynı mantıkta çalışıyor
            for(int i=0;i<str.Length;i++) 
            {
                 chr=Convert.ToChar(str.Substring(i,1));
                 if(chr=='+'||chr=='-')
                 {

                    previousNumber=findBeforeNumber(i,str.Substring(0,i));
                    nextNumber=findAfterNumber(str.Substring(i+1,str.Length-i-1));
                    if(chr=='+')
                    {                
                        result= previousNumber+nextNumber;
                        string strIslem=previousNumber.ToString()+"+"+nextNumber.ToString();
                        searchIndex=str.IndexOf(previousNumber+"+"+nextNumber,0);
                        firstString=str.Substring(0,searchIndex);
                        lastString=str.Substring(strIslem.Length+firstString.Length,str.Length-(firstString.Length+strIslem.Length));                
                        str=firstString+result+lastString;
                        i=0;
                    }
                    else if(chr=='-')
                    {
                        result= previousNumber-nextNumber;
                        string strIslem=previousNumber.ToString()+"-"+nextNumber.ToString();
                        searchIndex=str.IndexOf(previousNumber+"-"+nextNumber,0);
                        firstString=str.Substring(0,searchIndex);
                        lastString=str.Substring(strIslem.Length+firstString.Length,str.Length-(firstString.Length+strIslem.Length));
                        str=firstString+result+lastString;
                        i=0;
                        
                    }
                 }
            }

           

           
            
        return result;
        } 

        //buraya gelen işlem "20-3" biz buradan sadece 3 ü almak istiyoruz 
        public static int findBeforeNumber(int i,string str)
        {
            int result=0;
            char chr;
            string number="";

            //sondan başa doğru döngü oluşturduk
            for(int j=i;j>=0;j--) 
            {
                //buradaki amaç herhangi bir işlem sembolüyle karşılaşmazsak gelen ifade neyse aynısını göndermiş oluyoruz
                if(j-1==-1)
                {
                     number=str.Substring(j);
                     result= Convert.ToInt32(number);
                     return result;
                }
                //eğer farklı bir sembolle karşılaştıysak bu bizim işlemimize ait değil başka bir işlemin sembolüdür o yüzden biz aradığımızı bulmuş olduk
                 chr=Convert.ToChar(str.Substring(j-1,1));
                 if(chr=='/' || chr=='*' || chr== '+' || chr=='-')
                 {
                      /*eğer ki gelen ifade -15 gibi negatif bir sayı ise onu tespit etmek için kullanılıyor
                      
                      şöyle ki -15 ifadesi geldi parça parça bakıyoruz her karakterine 5 i gördük 1 i gördük 
                      sonra bakıyoruz ki bir işlem sembölüyle karşılaştık normalde * + / gibi bir sembol olsa bu bizi ilgilendirmiyordu 
                      - sembolü ayrı bir özelliği oluşuyor normalde 15-5 ifadesinde bu sembol problem yaratmaz çünkü 15 ilerde başka bir işleme girecek
                      fakat şöyle düşünün bu findBeforeNumber metoduna sadece -5 ifadesi gelmiş öncesinde başka bir işlem yok bu işlem negatif olacak
                      o yüzden bu if ifadesinde - den öncesinde bir karakter var mı diye kontrol etmek gerekiyor burada ona bakıyoruz eğer yoksa olduğu gibi gönderiyoruz
                      */
                     if(j-2==-1)      
                     {
                        number=str.Substring(j-1); 
                     }
                     else
                     {
                        number=str.Substring(j);                 
                     }
                     //sadece "3" ifadesini alıp geri dönüyoruz
                     result= Convert.ToInt32(number);
                     return result;                    
                 }
            }
        return result;
        } 

        //yukarıdakiyle aynı mantık bu sefer gelen ifade "15-1" soldaki 15 almak istiyoruz 
        public static int findAfterNumber(string str)
        {
            int result=0;
            char chr;
            string number="";
            for(int j=0;j<str.Length;j++)
            {
                if(j+1==str.Length)
                {
                     number=str.Substring(0,j+1);
                     result= Convert.ToInt32(number);
                     return result;
                     
                }
                 chr=Convert.ToChar(str.Substring(j,1));
                 if(chr=='/' || chr=='*' || chr== '+' || chr=='-')
                 {
                     number=str.Substring(0,j);
                     result= Convert.ToInt32(number);
                     return result;         
                 }
            }
            return result;
        } 
    }
}

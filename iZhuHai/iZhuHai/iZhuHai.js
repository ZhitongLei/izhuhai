// JavaScript Document
$(document).ready(
				  function(){
					  $("#applySub").click(
										   function(){
											   
											   //ºÏ≤È” œ‰
					                           var patten = new RegExp(/^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]+$/);
											   var emails=$("#email").val();
											   if(emails==""||!patten.test(emails)){
												   $("#alertEmail").show("slow");
												   return false;
												   }
												
											  var names=$("#names").val();
											  if(names==""||names.length<2){
												   $("#alertName").show("slow");
												   return false;
												  }
												  
											  var introduce=$("#introduce").val();
											  if(introduce==""){
												   $("#alertIntroduce").show("slow");
												   return false
												  }  
											  
											   }
										   )
					  
					   $("label").click(
										  function(){
											  $(".alerts").hide("slow");
											  }
										  
										  )
					  
					  }
				  )
$(document).ready(function(){

	function updateCharacterList()
	{
		if ( $('#characterList')  )
		{
			$.ajax({
				url: '/api/getCharacters',
				cache: false,
				success: function(data){
					var jsonObj=  eval('(' + data + ')');
					var items = [];

					$.each(jsonObj['characters'], function(key, val) {
						items.push('<li id="character' + val['id'] 
									+'">'+val['id']+ ' '+ val['name'] + ' HP' + val['hpp'] +'% TP'+ val['tp']
									 + 'Pos (' + val['x'] + ',' + val['y']+','+val['z']+')'
									 +'<button class="btn" onclick="startStrategyForCharacter('+ val['id'] + ',0);"></li>');
					});

					$('#characterList').empty();
					$('<ul/>', {
						'class': 'my-new-list',
						html: items.join('')
					  }).appendTo('#characterList');
				}
			});
		}
	}

	function updateMonstersList()
	{
		if ( $('#monstersList')  )
		{
			$.ajax({
				url: '/api/getMonsters',
				cache: false,
				success: function(data){
					var jsonObj=  eval('(' + data + ')');
					var items = [];

					$.each(jsonObj['monsters'], function(key, val) {
						items.push('<li id="monster' + val['id'] 
									+'">'+val['id']+ ' '+ val['name'] + ' HP' + val['hpp'] +'% TP'+ val['tp']
									 + 'Pos (' + val['x'] + ',' + val['y']+','+val['z']+')</li>');
					});

					$('#monstersList').empty();
					$('<ul/>', {
						'class': 'my-new-list',
						html: items.join('')
					  }).appendTo('#monstersList');
				}
			});
		}
	}

	function startStrategyForCharacter(characterId,strategyId)
	{
			$.ajax({
				url: '/api/startStrategy/?c=' +characterId + '&s=' + strategyId,
				cache: false,
				success: function(data){
					alert('Ok !');
				}
			});
	}

	setInterval(updateCharacterList,1000);
	setInterval(updateMonstersList,1000);
});
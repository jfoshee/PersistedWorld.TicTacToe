/**
 * Create new TicTacToeBoard
 */
function initialize(context) {
  let entity = context.entity;
  entity.displayName = 'TicTacToe Board';
  // The Author who authored this code and created this object. 
  // Only Author's state can be modified in custom function.
  let authorId = context.authorId;
  let state = entity.customStatePublic[authorId];
  state.type = 'TicTacToeBoard';
  state.board = [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '];
  state.isComplete = false;
  state.message = '';
}

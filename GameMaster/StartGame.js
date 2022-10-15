/** Start Game */
function mutate(context, opponentId) {
  if (!opponentId)
    throw Error(`Missing required argument: otherPlayerId`);
  const user = context.user;
  const entity = context.entity;
  const ownerId = entity.SystemState.OwnerId;
  if (user.id !== ownerId) {
    throw Error(`You cannot start a TicTacToe game that you don't own.`);
  }
  // TODO: If game is already complete throw error
  // Player 1: the one who owns the board
  var player1Id = user.id;
  // Player 2: passed in via custom argument
  var player2Id = opponentId;
  let state = entity.customStatePublic[context.authorId];
  // TODO: Factor out constants
  if (state.type !== 'TicTacToeBoard')
    throw Error(`StartGame should only be invoked on a TicTacToeBoard`);
  state.board = [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '];
  state.isComplete = false;
  state.turn = player1Id;
  state.players = [player1Id, player2Id];
  state.message = `The TicTacToe game has started. It is ${user.displayName}'s turn!`;
  log(state.message);
}
